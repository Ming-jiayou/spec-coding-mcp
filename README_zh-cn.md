# Spec Coding MCP 服务器

通过系统化的**规格驱动开发**，将功能想法转化为可投入生产的代码

## 🎯 规格驱动开发工作流程

### 阶段 1：功能定义

- 通过与用户交互了解功能细节

### 阶段 2：需求收集

- 使用 EARS 格式将功能想法转化为正式需求

### 阶段 3：设计文档

- 根据确认的需求设计技术架构

### 阶段 4：任务规划

- 将设计分解为独立的、可执行的编码任务

### 阶段 5：任务执行

- 按照计划实现代码


## 🚀 快速开始
### 配置 MCP 服务器

#### 使用代码方式

`
{
  "servers": {
    "SpecCodingMcpServer": {
      "type": "stdio",
      "command": "<SpecCodingMcpServer.exe 目录路径>",
      "args": []
    }
  }
}
`

#### 使用可执行文件方式
`
{
  "servers": {
    "SpecCodingMcpServer": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "<spec-coding-mcp\src\SpecCodingMcpServer 目录路径>"
      ]
    }
  }
}
`

#### 使用 NuGet 方式

- 访问 `NuGet.org` 并按 mcpserver 包类型筛选

- 搜索 `SpecCodingMcpServer` 并从 “MCP Server” 选项卡复制配置

### 启动工作流程

以下是使用**规格驱动开发**工作流程的完整示例：

#### 1. 功能定义
`
我：开始规格编码

AI：[调用 SpecCodingMcpServer/spec_coding_workflow_start]
欢迎来到规格编码！让我们从定义你的功能开始。

我：创建一个 Vue 待办应用

AI：为了确保我们构建出完全符合你需求的产品，我需要再明确一些细节

我：[输入关于该功能的更多细节]

AI：这样看起来可以继续了吗？请用“是”确认

我：是

AI：[调用 SpecCodingMcpServer/spec_coding_feature_confirmed]
[按照规格编码工作流程的要求创建功能文档目录和 README 文件]
`

#### 2. 需求收集

`
AI：开始 Vue 待办应用的需求收集阶段
[调用 SpecCodingMcpServer/spec_coding_requirements_start]
[创建具有详细 EARS 格式需求的 Vue 待办应用需求文档]
这些需求看起来可以吗？如果可以，我们可以进入设计阶段。

我：是

AI：[调用 SpecCodingMcpServer/spec_coding_requirements_confirmed]
`

#### 3. 设计文档

`
AI：开始 Vue 待办应用的设计阶段
[调用 SpecCodingMcpServer/spec_coding_design_start]
[根据需求创建 Vue 待办应用的全面设计文档]
这个设计看起来可以吗？如果可以，我们可以进入实施计划阶段。

我：是

AI：[调用 SpecCodingMcpServer/spec_coding_design_confirmed]
`

#### 4. 任务规划

`
AI：开始 Vue 待办应用实施的任务创建阶段
[调用 SpecCodingMcpServer/spec_coding_tasks_start]
[根据设计和需求创建实现 Vue 待办应用的全面任务列表]
这些任务看起来可以吗？请查看任务列表，在我们进入实施阶段之前，如果你需要任何调整，请告诉我。

用户：是

AI：[调用 SpecCodingMcpServer/spec_coding_tasks_confirmed]
`

#### 5. 任务执行

`
AI：开始 Vue 待办应用的实际实施阶段
[调用 SpecCodingMcpServer/spec_coding_execute_start]
[逐一执行任务]
`

#### 最终生成的文件夹结构
`
.spec-coding/features/vue-todo-app/
├── requirements.md   # EARS 格式的需求
├── README.md         # 功能描述
├── design.md         # 技术架构
└── tasks.md          # 实施清单
`