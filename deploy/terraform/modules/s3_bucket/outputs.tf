output "bucket_id" {
  description = "S3 bucket ID."
  value       = aws_s3_bucket.this.id
}

output "bucket_arn" {
  description = "S3 bucket ARN."
  value       = aws_s3_bucket.this.arn
}

output "bucket_name" {
  description = "S3 bucket name."
  value       = aws_s3_bucket.this.bucket
}

output "bucket_domain_name" {
  description = "Bucket domain name."
  value       = aws_s3_bucket.this.bucket_domain_name
}

output "bucket_regional_domain_name" {
  description = "Bucket regional domain name."
  value       = aws_s3_bucket.this.bucket_regional_domain_name
}

output "bucket_region" {
  description = "AWS region of the bucket."
  value       = data.aws_region.current.name
}

output "bucket_policy_json" {
  description = "Generated bucket policy JSON."
  value       = data.aws_iam_policy_document.bucket.json
}

output "attachment_base_url" {
  description = "Base S3 URL you can use to compose attachment object URLs."
  value       = "s3://${aws_s3_bucket.this.bucket}"
}
