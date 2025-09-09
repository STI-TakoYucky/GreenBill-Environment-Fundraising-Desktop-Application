using GreenBill.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBill.IServices
{
    public interface ISupportingDocumentService
    {
        Task Create(SupportingDocument supportingDocument);
    }
}
