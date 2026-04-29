using System.Collections.Generic;

namespace onlineShop.DTOs
{
    /// <summary>
    /// Result of a bulk import operation
    /// Contains statistics and details about imported products
    /// </summary>
    public class ImportResultDto
    {
        /// <summary>
        /// Number of products successfully created
        /// </summary>
        public int Success { get; set; }

        /// <summary>
        /// Number of products that failed to import due to errors
        /// </summary>
        public int Failed { get; set; }

        /// <summary>
        /// Number of products skipped (usually duplicates)
        /// </summary>
        public int Skipped { get; set; }

        /// <summary>
        /// Number of products updated (if update mode is enabled)
        /// </summary>
        public int Updated { get; set; }

        /// <summary>
        /// Summary message of the import operation
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// List of successfully imported products (Aref - Design)
        /// </summary>
        public List<string> SuccessfulProducts { get; set; } = new List<string>();

        /// <summary>
        /// List of skipped products (Aref - Design - Reason)
        /// </summary>
        public List<string> SkippedProducts { get; set; } = new List<string>();

        /// <summary>
        /// Detailed error information for failed imports
        /// </summary>
        public List<ImportErrorDto> Errors { get; set; } = new List<ImportErrorDto>();
    }

    /// <summary>
    /// Details of an import error for a specific product
    /// </summary>
    public class ImportErrorDto
    {
        /// <summary>
        /// Product reference (aref)
        /// </summary>
        public string Aref { get; set; }

        /// <summary>
        /// Product designation
        /// </summary>
        public string Design { get; set; }

        /// <summary>
        /// List of validation/import errors for this product
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();
    }
}