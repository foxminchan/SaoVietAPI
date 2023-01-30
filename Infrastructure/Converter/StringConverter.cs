using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Converter
{
    /**
    * @Project ASP.NET Core 7.0
    * @Author: Nguyen Xuan Nhan
    * @Team: 4FT
    * @Copyright (C) 2023 4FT. All rights reserved
    * @License MIT
    * @Create date Mon 23 Jan 2023 00:00:00 AM +07
    */

    public class StringConverter : ValueConverter<string, DateTime>
    {
        public StringConverter() : base(
            v => DateTime.Parse(v),
            v => v.ToString("dd/MM/yyyy")
        )
        {
        }
    }
}
