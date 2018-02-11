ITCS - 6166: Project 01
Authors: Taylor Conners, Stephen Stroud

Instructions:

	- This project requires .NET core, which can be installed
	from: https://www.microsoft.com/net/learn/get-started/windows
	
NOTE: Testing on this program was done by running cmd(Command Prompt) as Administrator. 
Issuing a CTRL+C termination command in Git Bash did not successfully stop
or close the HttpListener employed by the server, but the application
closes cleanly when issuing CTRL+C through cmd.

HTTP_Server:

	To run the server, navigate your terminal to the HTTP_Server directory 
	and type:
		
		- "dotnet run port_number" 
		
		Where port_number = the port number that you wish the server to be 
		listening for connections at.
		
	Files retrieved from the server are located in "...\HTTP_Server\contents\web",
	so any files requested from the server must be in this location.
	
	Terminating Server:
		
		From the terminal, press CTRL+C to terminate the application. The specified
		port will no longer be in use by the application.
	
HTTP_client:
	
	GET Request:
	
		To perform a get request, navigate your terminal to the 
		HTTP_client directory and type:
			
			- "dotnet run http://localhost port_number GET file_name"
			
		port_number must be equal to the port number at which the server
		is currently listening; an exception will be thrown otherwise.
			
		file_name must include the name AND extension of the file. For
		instance, if there is a file named 'index.html' on the server,
		file_name must be 'index.html' to retrieve that file.

	PUT Request:
		
		Files that are to be sent to the server must be located in
		"...\HTTP_client\files".
		
		To perform a PUT request, navigate your terminal to the HTTP_client
		directory and type:
		
			- "dotnet run http://localhost port_number PUT file_name"
			
		port_number must be equal to the port number at which the server
		is currently listening; an exception will be thrown otherwise.
			
		As with the GET request, file_name must include the file's name
		and extension as it appears in "...\HTTP_client\files"
			
		After a successful PUT, the file can be retrieved with a GET request
		from the server.