# Prerequisites
1. You must have the appropriate SSDT components installed for the version of SSIS you are targeting.
2. You must have the Client SDK components for SSIS installed (part of SQL Server).
3. You must run Visual Studio as an adminstrator.
4. The Gac Powershell module must be installed.
   1. Open Powershell as an administrator.
   2. Ensure execution of local scripts is allowed (Set-ExecutionPolicy -remoteSigned)
   3. Ensure the module is installed (Get-Module -ListAvailable -Name Gac)
   4. Install it if it is not already there (Install-Module Gac -Force)

For purposes of this document SQL 2016 will be used (13.0). Older or newer versions will have a different version number. Where it
says {SsisVersion} use the correct version number for the targeted SSIS version.

# Creating the Runtime Task

## Create the Runtime Component

1. Create a new project for the task (P3Net.IntegrationServices.Tasks.DoSomething).
      a. Project Type = Class Library
      b. Framework Version = 4.5.2 (do not go higher than this version).
      c. Sign the assembly using the SNK file in the solution.
      d. Copy the Build Events settings from P3Net.IntegrationServices into the project.
      e. Update the AssemblyVersion.cs file to contain appropriate information about the assembly. Remove the following attributes.
		  1. AssemblyVersion
		  2. AssemblyFileVersion
	  f. Add a shared file link to the Shared\AssemblyVersion.cs file.
2. Add references to the required assemblies.
      a. P3Net.IntegrationServices (Project)
      b. Microsoft.SqlServer.ManagedDTS {SsisVersion} (Framework\Extensions)
      c. System.Drawing (Framework)
      d. System.Runtime.Serialization (Framework)
3. Add a new class to represent the task (i.e. DoSomethingTask).
      a. Derive from ```BaseTask```.
      b. Make the task public.
4. Create an icon for the task.
      a. Add to the project (ensure Build Action is Embedded Resource).      
      b. Go to Resources editor and add a resource for the icon.
      c. Ensure the Resources generated file is Public.
5. In the task type add the standard properties that will be needed.
	```
	public const string Description = "Some description seen by the designer";
	public const string TaskName = "Display name of the task";
	public static Icon Icon = Resources.MyTaskIcon;
	public const string UITypeName = "P3Net.IntegrationServices.Tasks.MyTaskUI, P3Net.IntegrationServices.Tasks.MyTask.UI, Version=" + AssemblyMetadata.ProductVersion + ", Culture=Neutral, PublicKeyToken={Public key}";      
	```
6. Apply the ```DtsTask``` attribute
	```
	[DtsTask(DisplayName = MyTask.TaskName, RequiredProductLevel = DTSProductLevel.None,
	         Description = MyTask.Description, IconResource = "{path to icon resource}",
			 UITypeName = MyTask.UITypeName)]
	```			 
7. Implement the ```ExecuteCore``` method
      a. This method is called at runtime to perform the work of the task.
      b. Use the context parameter to access information needed by the task.    
      c. Return ```DTSExecResult.Success``` if the task completes or throw an exception otherwise.      
      d. Consider raising informational events to help diagnose issues at runtime.
8. (Optional) Implement ```ValidateCore```
      a. If the task needs to do any validation including verifying connections and that all properties have been set then implement this method.
      b. If any validation fails then use Events.LogError to log the error and return ```DTSExecResult.Failure```.
      c. Return the results of Base.ValidateCore if no validation errors occur.

## Adding Task Properties

For each property that will be configurable in SSIS do the following.

1. Create a public property with a getter and setter.
2. Optionally set a default value for the property
3. If the property will be using a connection from ConnectionManager then do the following.
      a. Create a private field to store the connection ID (i.e. _connectionId).
	  b. Create the getter/setter to use the private field
	  ```
	  public string MyConnection 
	  {
		  get { return TryGetConnectionName(_connectionId) ?? ""; }
		  set { _connectionId = TryGetConnectionId(value); }
	  }

## Working with Connections at Runtime

To use a connection at runtime do the following.

1. Use ```Connections.GetConnection``` to get the ConnectionManager.
2. Use ```Acquireconnection``` to get the underlying connection.
3. Pass the connection to a new connection type (i.e. HttpClientConnection) to get a usable instance.
4. If any changes will be made to the connection, including URL or credentials then use ```Clone()``` on the object returned by AcquireConnection.
	```
        var cm = context.Connections.GetConnection(ServerConnection);

        //Create a copy of the connection because we're going to change the URL
        var conn = new HttpClientConnection(cm.AcquireConnection(context.Transaction)).Clone();
        if (conn == null)
            throw new Exception("Unable to acquire connection.");
	```

## Working with Variables at Runtime

Variables are multi-threaded resources in SSIS. Accessing a variable requires that it be locked and unlocked.  Extension methods
are provided to hide the details and can be used in most cases.

1. Use ```Variables.TryGetInfo``` to get information about a variable.
2. Use ```Variables.GetVar<T>``` or ```TryGetVar<T>``` to safely read a variable's value. Note that the value may change after the get returns.
3. Use ```Variables.SetVar<T>``` to safely set a variable's value.
4. If a lock needs to be held longer than a get/set call then use the standard locking API.

## Implementing Persistence
Every property of a task has to be persisted to XML. The default implementation will handle this for public properties of
primitive or string types. For all other types including arrays and lists persistence has to be manually implemented.

1. Add the ```IDTSComponentPersist``` interface to the task.
2. Implement ```LoadXML```
      a. Use the extension methods and XML API to get each attribute and element from the XML.
      b. Remember that new properties may not have been persisted so handle this case gracefully.
		 ```
        public void LoadFromXML ( XmlElement node, IDTSInfoEvents infoEvents )
        {
            Content = node.GetAttributeValue("Content");
            ReportFormat = node.GetAttributeValue("ReportFormat");
            ReportPath = node.GetAttributeValue("ReportPath");
            m_connectionId = node.GetAttributeValue("ServerConnection");

            var elements = node.SelectNodes("Arguments/Argument").OfType<XmlElement>();
            foreach (var element in elements)
            {
                var arg = new ReportParameter()
                {
                    Name = element.GetAttributeValue("name"),
                    Type = (ReportParameterType)Enum.Parse(typeof(ReportParameterType), element.GetAttributeValue("type")),
                    IsNullable = Boolean.Parse(element.GetAttributeValue("isNullable")),
                    DefaultValue = element.GetAttributeValue("defaultValue"),
                    Value = element.GetAttributeValue("value")
                };

                Arguments.Add(arg);
            };
        }
		 ```
3. Implement ```SaveXML```
      a. Use the extension methods and XML API to store each property in an attribute or element.  Use attributes for simple types and elements for complex types.
      b. The properties must be stored in a child element.
		```
        public void SaveToXML ( XmlDocument doc, IDTSInfoEvents infoEvents )
        {
            var root = doc.CreateAndAddElement(GetType().Name);

            root.SetAttributeValue("Content", Content);
            root.SetAttributeValue("ReportFormat", ReportFormat);
            root.SetAttributeValue("ReportPath", ReportPath);
            root.SetAttributeValue("ServerConnection", m_connectionId);

            var element = root.CreateAndAddElement("Arguments");
            foreach (var arg in Arguments)
            {
                var argumentElement = element.CreateAndAddElement("Argument");
                argumentElement.SetAttributeValue("name", arg.Name);
                argumentElement.SetAttributeValue("type", arg.Type);
                argumentElement.SetAttributeValue("isNullable", arg.IsNullable);
                if (!String.IsNullOrWhiteSpace(arg.DefaultValue))
                    argumentElement.SetAttributeValue("defaultValue", arg.DefaultValue);
                argumentElement.SetAttributeValue("value", arg.Value);
            };
        } 
		```

# Creating the Design Time Task

## Creating the Design Time Component

1. Create a new project for the task (P3Net.IntegrationServices.Tasks.DoSomething.UI).
      a. Project Type = Class Library
      b. Framework Version = 4.5.2 (do not go higher than this version).
      c. Sign the assembly using the SNK file in the solution.
      d. Copy the Build Events settings from P3Net.IntegrationServices.UI into the project.
      e. Update the AssemblyVersion.cs file to contain appropriate information about the assembly.
	  e. Update the AssemblyVersion.cs file to contain appropriate information about the assembly. Remove the following attributes.
		  1. AssemblyVersion
		  2. AssemblyFileVersion
	  f. Add a shared file link to the Shared\AssemblyVersion.cs file.
2. Add references to the required assemblies.
      a. P3Net.IntegrationServices (Project)
      b. P3Net.IntegrationServices.UI (Project)
      c. The runtime assembly for the task (Project)
      d. Microsoft.SqlServer.Dts.Design {SsisVersion} (Framework\Extensions)
      e. Microsoft.SqlServer.ManagedDTS {SsisVersion} (Framework\Extensions)
      e. System.Drawing (Framework)
      f. System.Runtime.Serialization (Framework)
      g. System.Windows.Forms (Framework)
      h. Microsoft.DataTransformationServices.Controls (Browse -> %Windows%\Microsoft.NET\assembly\GAC_MSIL\Microsoft.DataTransformationServices.Controls\v4.0_11.0)

## Creating the Task Form

1. Create a new, internal WinForms form class (eg. MyTaskForm).
2. Set the following properties.
     a. Text = My Task Editor
3. Change the form to derive from ```DTSBaseTaskUI```.
4. Modify the constructor to accept at least the ```TaskHost``` host and ```Object``` representing the connections.
5. Modify the constructor to pass the task name, icon, description, host and connection objects to the base class (these values can come from the constant properties created for the runtime task).
6. (Recommended) Pass true as the last parameter so the user has access to the Expressions tab.
7. In the constructor add each task view instance to the host.
	```
    var startView = new GeneralView();

    DTSTaskUIHost.FastLoad = false;      
    DTSTaskUIHost.AddView("General", startView, null);
    DTSTaskUIHost.AddView("Settings", new SettingsView(), null);
    DTSTaskUIHost.FastLoad = true;      

    DTSTaskUIHost.SelectView(startView);
	```

## Creating the Task Views (Properties)

Each tab in the designer is a separate view on the form. Many views can be implemented by using a ```PropertyGrid```. For these types of views do the following.

1. Create a new type, internal to represent the view data (eg. GeneralViewNode).
2. Add the ```SortProperties``` attribute to configure the order in which properties are shown.
3. In the constructor initialize any needed fields.
4. For each property that is to be exposed.
      a. Add a public property with appropriate default value.
      b. Add the ```Category``` attribute to group the properties in the UI.
      c. Add the ```Description``` attribute to provide help for the property.

1. Create a new, internal type to represent the view (i.e. GeneralView).
2. Derive the view from ```DtsTaskUIPropertyView<TNode>``` where ```TNode``` is the view data type.
3. Override ```CreateNode``` to create the view data.
4. (Optional) Implement ```OnInitializeCore``` to do any view-specific initialization.
      a. Ensure that you call ```Base.OnInitializeCore``` before accessing any Node data.
      b. This method is called only once so only do one-time initialization in it.
5. (Optional) Implement ```OnPropertyChanged``` if the view needs to do something when a property value is changed.
6. Implement ```Save``` to save the changes back to the task.
	```
    protected override void Save ( )
    {
        var task = GetTask<MyTask>();

        //General properties
        Host.Name = Node.Name;
        Host.Description = Node.Description;

        //Task properties
        task.Property1 = Node.Property1;
        ...
    }
	```
7. (Optional) Implement ```OnSelectionCore``` to handle any per-selection initialization.
      a. Updates to the UI based upon changes in other views should be done here.
      b. Use ```GetView<T>``` to get access to data in other views that impact what is displayed in the view.  Use ```GetTask``` only for initialization.

## Creating the Task Views (Non-Properties)

For views that will not use a ```PropertyGrid``` additional work is needed.

1. Create a new, internal type to represent the view (i.e. SettingsView).
2. Derive the type from ```DtsTaskUIView``` (note that the designer will no longer render the form after this change).
3. Implement the type as you would any form.
4. Override ```OnInitializeCore``` to initialize the view controls based upon the associated task.
5. Override ```Save``` to save the view contents back to the associated task.
6. (Optional) Implement ```OnSelectionCore``` to do any per-selection initialization.
      a. Updates to the UI based upon changes in other views should be done here.
      b. Use ```GetView<T>``` to get access to data in other views that impact what is displayed in the view.  Use ```GetTask``` only for initialization.
 
## Creating the Task UI

1. Create a new, public class that derives from ```DtsTaskUI``` and represents the UI for the task (eg. MyTaskUI).
2. Implement the ```GetViewCore``` method to return an instance of the task form created earlier.
	```
    protected override ContainerControl GetViewCore ()
    {
        return new GenerateSsrsTaskForm(Host, ServiceProvider.GetService<IDtsConnectionService>());
    }
	```

4. On the ```DtsAttribute``` for the runtime task add a ```UITypeName``` property and set it to the type just created.

## Supporting Variables at Design Time

If properties shown in the UI should be variables then additional work is needed to make the integration seamless.

1. Apply a type converter attribute to the property in the view data type and use the ```VariablesStringConverter``` type.
2. (Optional) Apply the ```DefaultVariable``` attribute to the property to specify the default name and type of the new variable. 
3. (Alternative) If the defaults are more complex than can be specified in the attribute then implement the ```INewVariableProvider``` interface.
4. The base type will handle the creation and selection of the new variable.

## Supporting Connections at Design Time

If properties shown in the UI should be connections then additional work is needed to make the integration seamless.

1. Apply a type converter attribute to the property in the view data type and use a converter that is specific to the type of connection(s) that are supported (see Converters for existing types).
2. The base type will handle the creation and selection of the new connection.

# Debugging

## Debugging the Runtime Component

To debug the runtime side of the task do the following.

1. Build the code and ensure it was properly registered.
2. Start SSDT and load an SSIS project.
3. Drag and drop the task to the designer and set any properties.
4. Save the project.
5. Set a breakpoint on the first task in the designer.
6. Start the SSIS package.
7. When the breakpoint is hit switch back to VS and Attach to Process -> DtsDebugHost.exe (Managed x64 version).
8. Set breakpoints in the task code accordingly.
9. Continue running the SSIS package.

## Debugging the Design Component

1. Build the code and ensure it was properly registered.
2. Restart SSDT if it is already running.
3. Load an SSIS project.
4. Drag and drop the task to the designer and save the project.
5. Switch back to VS and Attach to Process -> DevEnv.exe.
6. Set any breakpoints desired.
7. Use SSDT to open the task UI for debugging.
