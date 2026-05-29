using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Cpenel
{
    public class ResultDto
    {
        public bool IsSucces { get; set; }
        public string Message { get; set; }
    }
    public class ResultUploadDto : ResultDto
    {
        public string Filename { get; set; }
    }
    public class ResultTableDto<T> : ResultDto
    {
        public T? Value { get; set; }
        public List<T>? Values { get; set; }

    }
}
