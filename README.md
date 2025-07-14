# Email Reader and MCP Server

This solution contains two projects: an Email Reader service and a Model Context Protocol (MCP) server.

## Technologies Used

- .NET 9
- C#
- MailKit
- Elasticsearch
- MCP

## Projects

### EmailReader

This project is a service that reads emails from an IMAP server and indexes them into an Elasticsearch instance.

### McpServer

This project is a server that implements the Model Context Protocol (MCP). It uses an Elasticsearch instance to search for emails.

### EmailReader.Tests

This project contains unit tests for the EmailReader project.

### McpServer.Tests

This project contains unit tests for the McpServer project.

## How to Run

### Docker Compose

To run the solution locally, you can use Docker Compose. This will start the McpServer and an Elasticsearch instance.

```bash
docker-compose up
```

### Add Local MCP Server to VS Code

To add the local MCP server to VS Code, you can follow the instructions here: https://code.visualstudio.com/docs/copilot/chat/mcp-servers

You will need to add the following to your VS Code settings:

```json
"copilot.mcp.servers": {
    "my-local-mcp-server": {
        "url": "http://localhost:5000"
    }
}
```

### Run Email Reader

To run the Email Reader service, you will need to set the following environment variables:

```bash
export Imap__Host="your-imap-host"
export Imap__Port="your-imap-port"
export Imap__UseSsl="true"
export Imap__Username="your-imap-username"
export Imap__Password="your-imap-password"
export Imap__Folders__0="Inbox"
export Elasticsearch__Url="http://localhost:9200"
```

Then, you can run the service with the following command:

```bash
dotnet run --project EmailReader/EmailReader.csproj
```
