 **[简体中文](./README_zh-cn.md)**
 
# Spec Coding MCP Server

Transform feature ideas into production-ready code through systematic **Spec-Driven Development**

## 🎯 Spec-Driven Development Workflow

### Phase 1: Feature Definition

- Learn details about feature by interacting with user

### Phase 2: Requirements Gathering

- Transform feature idea into formal requirement using EARS format

### Phase 3: Design Documentation 

- Develop technical architecture based on approved requirements

### Phase 4: Task Planning  

- Break down design into discrete, actionable coding tasks

### Phase 5: Task Execution

- Implement the code following the plan

## 🚀 Quick Start

### Config MCP Server

#### Using exe

```json
{
  "servers": {
    "SpecCodingMcpServer": {
      "type": "stdio",
      "command": "<PATH TO SpecCodingMcpServer.exe DIRECTORY>",
      "args": []
    }
  }
}
```

#### Using code

```json
{
  "servers": {
    "SpecCodingMcpServer": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "<PATH TO spec-coding-mcp\src\SpecCodingMcpServer DIRECTORY>"
      ]
    }
  }
}
```

#### Using NuGet

- Visit `NuGet.org` and filter by mcpserver package type
- Search `SpecCodingMcpServer` and copy the configuration from the “MCP Server” tab

### Start the Workflow

Here's a complete example of using the Spec Coding workflow:

#### 1. Feature Definition
```
Me: Start Spec Coding

AI: [call SpecCodingMcpServer/spec_coding_workflow_start]
Welcome to Spec Coding! Let's start by defining your feature.

Me: create a Vue todo app

AI：Let me clarify a few more details to ensure we build exactly what you need

Me: [enter more details for the feature]

AI：Does this look good to proceed? Please confirm with "yes"

Me: yes

AI：[call SpecCodingMcpServer/spec_coding_feature_confirmed]
[Create the feature documentation directory and README file as required by the spec coding workflow]
```
#### 2. Requirements Gathering
```
AI: Starting the requirements gathering phase for the Vue todo app
[call SpecCodingMcpServer/spec_coding_requirements_start]
[Create the requirements document for the Vue todo app with detailed EARS format requirements]
Do the requirements look good? If so, we can move on to the design phase.

Me: yes

AI: [call SpecCodingMcpServer/spec_coding_requirements_confirmed]
```

#### 3. Design Documentation
```
AI: Starting the design phase for the Vue todo app
[call SpecCodingMcpServer/spec_coding_design_start]
[Create a comprehensive design document for the Vue todo app based on the requirements]
Does the design look good? If so, we can move on to the implementation plan.

Me: yes

AI: [call SpecCodingMcpServer/spec_coding_design_confirmed]
```

#### 4. Task Planning
```
AI: Starting the task creation phase for the Vue todo app implementation
[call SpecCodingMcpServer/spec_coding_tasks_start]
[Create a comprehensive task list for implementing the Vue todo app based on the design and requirements]
Do the tasks look good? Please review the task list and let me know if you'd like any adjustments before we proceed to the implementation phase.

User: yes

AI: [call SpecCodingMcpServer/spec_coding_tasks_confirmed]
```

#### 5. Task Execution
```
AI: Starting the actual implementation phase for the Vue todo app
[call SpecCodingMcpServer/spec_coding_execute_start]
[Execute tasks one by one]
```

#### Generated Folder Structure
```
.spec-coding/features/vue-todo-app/
├── requirements.md   # EARS-format requirements
├── README.md         # Feature Description
├── design.md         # Technical architecture
└── tasks.md          # Implementation checklist
```
