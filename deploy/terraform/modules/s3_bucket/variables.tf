variable "bucket_name" {
  type        = string
  description = "Globally unique S3 bucket name used to store user attachments."
}

variable "force_destroy" {
  type        = bool
  description = "Whether Terraform may delete the bucket even when it contains objects."
  default     = false
}

variable "versioning_enabled" {
  type        = bool
  description = "Whether S3 versioning is enabled."
  default     = true
}

variable "object_ownership" {
  type        = string
  description = "S3 object ownership mode."
  default     = "BucketOwnerEnforced"

  validation {
    condition     = contains(["BucketOwnerEnforced", "BucketOwnerPreferred", "ObjectWriter"], var.object_ownership)
    error_message = "object_ownership must be BucketOwnerEnforced, BucketOwnerPreferred, or ObjectWriter."
  }
}

variable "sse_algorithm" {
  type        = string
  description = "Server-side encryption algorithm."
  default     = "AES256"

  validation {
    condition     = contains(["AES256", "aws:kms"], var.sse_algorithm)
    error_message = "sse_algorithm must be AES256 or aws:kms."
  }
}

variable "kms_key_arn" {
  type        = string
  description = "KMS key ARN used when sse_algorithm is aws:kms."
  default     = null
}

variable "bucket_key_enabled" {
  type        = bool
  description = "Whether to enable bucket keys for SSE-KMS."
  default     = true
}

variable "allowed_role_arns" {
  type        = list(string)
  description = "IAM role ARNs allowed to read and write attachment objects."
  default     = []
}

variable "allow_only_from_vpc_endpoint_id" {
  type        = string
  description = "Optional VPC endpoint ID allowed to access this bucket."
  default     = null
}

variable "attach_bucket_policy" {
  type        = bool
  description = "Whether the module should attach the generated bucket policy."
  default     = true
}

variable "cors_rules" {
  type = list(object({
    allowed_headers = list(string)
    allowed_methods = list(string)
    allowed_origins = list(string)
    expose_headers  = optional(list(string), [])
    max_age_seconds = optional(number, 3000)
  }))
  description = "Optional CORS rules for browser-based uploads or downloads."
  default     = []
}

variable "lifecycle_rules" {
  type = list(object({
    id                                = string
    enabled                           = bool
    prefix                            = optional(string, null)
    expiration_days                   = optional(number, null)
    noncurrent_version_expiration_days = optional(number, null)
  }))
  description = "Optional lifecycle rules for attachments."
  default     = []
}

variable "tags" {
  type        = map(string)
  description = "Tags applied to the bucket."
  default     = {}
}
