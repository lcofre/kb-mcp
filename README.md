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
