using System.Collections.Generic;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors
{
    public static class BrandDtoAdapter
    {
        public static BrandDataDto FromBrand(IBrand brand)
        {
            var brandDefinition = brand.BrandDefinition;
            var columns = new List<ColumnDto>();
            foreach (var column in brand.Columns)
            {
                var columnDefinition = brandDefinition.Columns.FirstOrDefault(h => column.Key == h.Key);
                columns.Add(FromColumn(column, columnDefinition));
            }
            return new BrandDataDto
            {
                Columns = columns
            };
        }

        private static ColumnDto FromColumn(IColumnData column, ISimpleColumnDefinition columnDefinition)
        {
            var columnValue = BrandDataTypeDtoAdaptor.ColumnValue(columnDefinition, column);
            return new ColumnDto
            {
                Value = columnValue,
                Name = columnDefinition.Name,
                Key = columnDefinition.Key,
            };
        }
    }
}