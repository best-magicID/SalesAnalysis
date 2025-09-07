using System.ComponentModel.DataAnnotations;

namespace SalesAnalysis.Models
{
    /// <summary>
    /// Месяц
    /// </summary>
    public class Month
    {
        #region ПОЛЯ И СВОЙСТВА

        /// <summary>
        /// Id месяца
        /// </summary>
        [Key]
        public int IdMonth { get; set; }

        /// <summary>
        /// Название месяца
        /// </summary>
        public string NameMonth { get; set; } = string.Empty;

        #endregion
    }
}
