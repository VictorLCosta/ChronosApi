variable "family" {
  type        = string
  description = "Task definition family name."
}

variable "container_name" {
  type        = string
  description = "Primary application container name."
}

variable "image" {
  type        = string
  description = "Container image URI."
}

variable "container_port" {
  type        = number
  description = "Application container port."
}

variable "cpu" {
  type        = string
  description = "Task CPU units."
  default     = "256"
}

variable "memory" {
  type        = string
  description = "Task memory in MiB."
  default     = "512"
}

variable "requires_compatibilities" {
  type        = list(string)
  description = "Task compatibilities."
  default     = ["FARGATE"]
}

variable "execution_role_arn" {
  type        = string
  description = "Execution role ARN for the task definition."
}

variable "task_role_arn" {
  type        = string
  description = "Task role ARN for the application."
}

variable "environment" {
  type        = map(string)
  description = "Plaintext environment variables passed to the container."
  default     = {}
}

variable "secrets" {
  type        = map(string)
  description = "Secrets Manager or SSM ARNs passed to the container."
  default     = {}
}

variable "aws_region" {
  type        = string
  description = "AWS region used by the log driver."
}

variable "create_log_group" {
  type        = bool
  description = "Whether the module should create the CloudWatch log group."
  default     = true
}

variable "log_group_name" {
  type        = string
  description = "Optional log group name. If null, one is generated."
  default     = null
}

variable "log_retention_in_days" {
  type        = number
  description = "Log retention in days."
  default     = 30
}

variable "log_group_kms_key_id" {
  type        = string
  description = "Optional KMS key for CloudWatch logs."
  default     = null
}

variable "log_stream_prefix" {
  type        = string
  description = "CloudWatch log stream prefix."
  default     = "ecs"
}

variable "readonly_root_filesystem" {
  type        = bool
  description = "Whether to enable read-only root filesystem."
  default     = false
}

variable "init_process_enabled" {
  type        = bool
  description = "Whether to enable init process inside the container."
  default     = true
}

variable "operating_system_family" {
  type        = string
  description = "Operating system family for the task runtime platform."
  default     = "LINUX"
}

variable "cpu_architecture" {
  type        = string
  description = "CPU architecture for the task runtime platform."
  default     = "X86_64"
}

variable "tags" {
  type        = map(string)
  description = "Tags applied to task resources."
  default     = {}
}
