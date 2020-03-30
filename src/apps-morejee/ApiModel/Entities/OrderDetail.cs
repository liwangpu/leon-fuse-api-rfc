using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiModel.Entities
{
    public class OrderDetail : EntityBase, IListable, IDTOTransfer<OrderDetailDTO>
    {
        public string ProductSpecId { get; set; }
        public int Num { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string Remark { get; set; }

        public string AttachmentIds { get; set; }
        public Order Order { get; set; }

        [NotMapped]
        public string Icon { get; set; }
        [NotMapped]
        public string ProductId { get; set; }
        [NotMapped]
        public ProductSpec ProductSpec { get; set; }
        [NotMapped]
        public string ProductCategoryName { get; set; }
        [NotMapped]
        public List<OrderDetailAttachment> Attachments { get; set; }

        public OrderDetailDTO ToDTO()
        {
            var dto = new OrderDetailDTO();
            dto.Id = Id;
            dto.ProductSpecId = ProductSpecId;
            dto.Num = Num;
            dto.UnitPrice = UnitPrice;
            dto.TotalPrice = TotalPrice;
            //dto.OrderDetailStateId = OrderDetailStateId;
            dto.Remark = Remark;
            dto.Creator = Creator;
            dto.Modifier = Modifier;
            dto.CreatedTime = CreatedTime;
            dto.ModifiedTime = ModifiedTime;
            dto.AttachmentIds = AttachmentIds;
            dto.Attachments = Attachments;
            if (ProductSpec != null)
            {
                dto.ProductSpecName = ProductSpec.Name;
                if (ProductSpec.Product != null)
                {
                    dto.ProductId = ProductSpec.Product.Id;
                    dto.ProductName = ProductSpec.Product.Name;
                    dto.ProductDescription = ProductSpec.Product.Description;
                    dto.ProductUnit = ProductSpec.Product.Unit;
                    dto.ProductCategoryId = ProductSpec.Product.CategoryId;
                    dto.ProductCategoryName = ProductSpec.Product.CategoryName;
                }
                dto.Icon = ProductSpec.Icon;
            }

            return dto;
        }
    }

    public class OrderDetailDTO : EntityBase
    {
        public string Icon { get; set; }
        public string ProductSpecId { get; set; }
        public int Num { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string Remark { get; set; }
        public string ProductId { get; set; }
        public string ProductCategoryId { get; set; }
        public string ProductName { get; set; }
        public string ProductCategoryName { get; set; }
        public string ProductUnit { get; set; }
        public string ProductDescription { get; set; }
        public string ProductSpecName { get; set; }
        public string AttachmentIds { get; set; }
        public List<OrderDetailAttachment> Attachments { get; set; }
    }

    public class OrderDetailAttachment
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }

}
