variable "name" {
  type = string
  description = "ECS cluster name"

  validation {
    condition     = can(regex("^[a-zA-Z0-9-_]+$", var.name))
    error_message = "Cluster name must contain only alphanumeric characters, hyphens, and underscores."
  }
}

variable "container_insights" {
  type        = bool
  description = "Enable CloudWatch Container Insights for the cluster."
  default     = true
}

variable "capacity_providers" {
  type        = list(string)
  description = "List of capacity providers to associate with the cluster."
  default     = ["FARGATE", "FARGATE_SPOT"]
}

variable "default_capacity_provider_strategy" {
  type = list(object({
    capacity_provider = string
    weight            = number
    base              = optional(number, 0)
  }))
  description = "Default capacity provider strategy for the cluster."
  default = [
    {
      capacity_provider = "FARGATE"
      weight            = 1
      base              = 1
    }
  ]
}