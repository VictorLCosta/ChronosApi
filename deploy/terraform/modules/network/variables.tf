variable "cidr_block" {
  type = string
  description = "CIDR block for the VPC."

  validation {
    condition = can(cidrhost(var.cidr_block, 0))
    error_message = "Must be a valid CIDR block."
  }
}

variable "public_subnets" {
  description = "Map of public subnet definitions."
  type = map(object({
    cidr_block = string
    az         = string
  }))

  validation {
    condition     = length(var.public_subnets) >= 1
    error_message = "At least one public subnet must be defined."
  }
}

variable "private_subnets" {
  description = "Map of private subnet definitions."
  type = map(object({
    cidr_block = string
    az         = string
  }))

  validation {
    condition     = length(var.private_subnets) >= 1
    error_message = "At least one private subnet must be defined."
  }
}

variable "tags" {
  type        = map(string)
  description = "Tags to apply to networking resources."
  default     = {}
}

variable "enable_flow_logs" {
  type        = bool
  description = "Enable VPC Flow Logs."
  default     = false
}