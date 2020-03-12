namespace WebAPP_LIULI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;

    public class MainModel : DbContext
    {
        //您的上下文已配置为从您的应用程序的配置文件(App.config 或 Web.config)
        //使用“MainModel”连接字符串。默认情况下，此连接字符串针对您的 LocalDb 实例上的
        //“WebAPP_LIULI.Models.MainModel”数据库。
        // 
        //如果您想要针对其他数据库和/或数据库提供程序，请在应用程序配置文件中修改“MainModel”
        //连接字符串。
        public MainModel()
            : base("name=MainModel")
        {
        }

        //为您要在模型中包含的每种实体类型都添加 DbSet。有关配置和使用 Code First  模型
        //的详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=390109。

        public virtual DbSet<Driver> MaterialDriver { get; set; }
        public virtual DbSet<MaterialOrder> MaterialOrder { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<QualityInspection> QualityInspection { get; set; }
        public virtual DbSet<Warehousing> Warehousing { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<SendOrder> SendOrder { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<TeamAllocation> TeamAllocation { get; set; }
        public virtual DbSet<WaterTeamAllocation> WaterTeamAllocation { get; set; }
        public virtual DbSet<RewardPunishment> RewardPunishment { get; set; }
        public virtual DbSet<BaseData> BaseData { get; set; }
        public virtual DbSet<HalfQualityInspection> HalfQualityInspection { get; set; }
        public virtual DbSet<HalfWarehousing> HalfWarehousing { get; set; }
        public virtual DbSet<MidSendOrder> MidSendOrder { get; set; }

    }

    public class Driver
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string CarNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string CarType { get; set; }
        public string MaxLoad { get; set; }
        public string PersonId { get; set; }
        public string BankNumer { get; set; }
        public string BankName { get; set; }
        public DateTime CreateTime { get; set; }
        public string Remarks { get; set; }
        public int Status { get; set; }
        //public virtual ICollection<MaterialOrder> Materials { get; set; }
        //public virtual ICollection<SendOrder> SendOrders { get; set; }
    }

    public class MaterialOrder
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string TaskId { get; set; }
        public double SendCount { get; set; }
        public double ReceiveCount { get; set; }
        public string SendDeterminePerson { get; set; }
        public string ReceiveDeterminePerson { get; set; }
        public string TaskStatus { get; set; }
        public string Remarks { get; set; }
        public DateTime? SendTime { get; set; }
        public DateTime? ReceiveTime { get; set; }
        public DateTime CreateTime { get; set; }
        public virtual Driver MaterialDriver { get; set; }
        public double OneOfTonPrice { get; set; }
        public int IsComfirm { get; set; }

    }

    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserType { get; set; }
        public string UserPermission { get; set; }
        public string UserTeam { get; set; }
        public string UserPhone { get; set; }
        public DateTime CreateTime { get; set; }
        public string DingUserId { get; set; }
        public string Remarks { get; set; }

    }

    public class QualityInspection
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ProcessName { get; set; }
        public double ScrapCount { get; set; }
        public string CheckTeam { get; set; }
        public string CheckResult { get; set; }
        public string UnqualifiedReson { get; set; }
        public string Remarks { get; set; }
        public string CheckUserName { get; set; }
        public DateTime CreateTime { get; set; }
        public string ProductName { get; set; }

    }

    public class HalfQualityInspection
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ProcessName { get; set; }
        public double ScrapCount { get; set; }
        public string CheckTeam { get; set; }
        public string CheckResult { get; set; }
        public string UnqualifiedReson { get; set; }
        public string Remarks { get; set; }
        public string CheckUserName { get; set; }
        public DateTime CreateTime { get; set; }

    }

    public class Warehousing
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ProductName { get; set; }
        public double InspectionCount { get; set; }
        public string InspectionUserName { get; set; }
        public DateTime? InspectionTime { get; set; }
        public double WarehousingCount { get; set; }
        public string WarehousingName { get; set; }
        public string WarehousingTeam { get; set; }
        public DateTime? WarehousingTime { get; set; }
        public string Remarks { get; set; }
        public DateTime CreateTime { get; set; }
        public int IsConfirm { get; set; }
        public string ConfirmName { get; set; }
        public string CustomerShortName { get; set; }

    }

    public class HalfWarehousing
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ProductName { get; set; }
        public double InspectionCount { get; set; }
        public string InspectionUserName { get; set; }
        public DateTime? InspectionTime { get; set; }
        public double HalfWarehousingCount { get; set; }
        public string HalfWarehousingName { get; set; }
        public string HalfWarehousingTeam { get; set; }
        public DateTime? HalfWarehousingTime { get; set; }
        public string Remarks { get; set; }
        public DateTime CreateTime { get; set; }
        public int IsConfirm { get; set; }
        public string ConfirmName { get; set; }

    }

    public class Customer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerShortName { get; set; }
        public string CustomerCompanyNumber { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }
        public string Contact { get; set; }
        public string ContactPhone { get; set; }
        public DateTime CreateTime { get; set; }
        public string Remarks { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

    }

    public class Order
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string TaskId { get; set; }
        public DateTime DeliveryTime { get; set; }
        public string ProductName { get; set; }
        public double ProductCount { get; set; }
        public string OrderStatus { get; set; }
        public string Salesman { get; set; }
        public double OrderPrice { get; set; } 
        public string Remarks { get; set; }
        public DateTime CreateTime { get; set; }
        public virtual Customer Customer { get; set; }
        public double DeliveryCount { get; set; }
    }

    public class MidSendOrder
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string TaskId { get; set; }
        public double SendCount { get; set; }
        public double ReceiveCount { get; set; }
        public string SendDeterminePerson { get; set; }
        public string ReceiveDeterminePerson { get; set; }
        public string TaskStatus { get; set; }
        public string Remarks { get; set; }
        public DateTime? SendTime { get; set; }
        public DateTime? ReceiveTime { get; set; }
        public DateTime CreateTime { get; set; }
        public string CustomerAddress { get; set; }
        public string Contact { get; set; }
        public string ContactPhone { get; set; }
        public virtual Order Order { get; set; }
        public double OneOfTonPrice { get; set; }
        public int IsComfirm { get; set; }



    }

    public class SendOrder
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string TaskId { get; set; }
        public double SendCount { get; set; }
        public double ReceiveCount { get; set; }
        public string SendDeterminePerson { get; set; }
        public string ReceiveDeterminePerson { get; set; }
        public string TaskStatus { get; set; }
        public string Remarks { get; set; }
        public DateTime? SendTime { get; set; }
        public DateTime? ReceiveTime { get; set; }
        public DateTime CreateTime { get; set; }
        public virtual Driver MaterialDriver { get; set; }
        public string CustomerAddress { get; set; }
        public string Contact { get; set; }
        public string ContactPhone { get; set; }
        public virtual MidSendOrder MidSendOrder { get; set; }
        public double OneOfTonPrice { get; set; }
        public int IsComfirm { get; set; }



    }


    public class Product
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ProductName { get; set; }
        public double OneOfWeight { get; set; }
        public double OneOfPrice { get; set; }
        public string Remarks { get; set; }
    }

    public class TeamAllocation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Date { get; set; }
        public double FixedAllocation { get; set; }
        public double Allocation { get; set; }
        public virtual User User { get; set; }
        public DateTime CreateTime { get; set; }
        public string Remarks { get; set; }
        public string UserName { get; set; }

    }

    public class WaterTeamAllocation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Date { get; set; }
        public double FixedAllocation { get; set; }
        public double Allocation { get; set; }
        public virtual User User { get; set; }
        public DateTime CreateTime { get; set; }
        public string Remarks { get; set; }
        public string UserName { get; set; }

    }

    public class RewardPunishment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Date { get; set; }
        public string Type { get; set; }
        public string Team { get; set; }
        public double Count { get; set; }
        public DateTime CreateTime { get; set; }
        public string Remarks { get; set; }
        public string UserName { get; set; }
    }

    public class BaseData
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public double Name { get; set; }
        public double AllocationRate { get; set; }
        public double OneOfTonPrice { get; set; }
        public double MonitorMoney { get; set; }
        public double HalfMoney { get; set; }
    }

}