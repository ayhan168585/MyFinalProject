ÇOK KATMANLI MİMARİYLE ETİCARET PROJESİ (ENGİN DEMİROĞ'UN PROJESİ)

# MyFinalProject
PROJE SIRALAMASI
-------------------------------------------------
C# Blank Solution oluşturulur.Projenin uygulama ismi verilir(MyFinalProject)
Class Library olarak Entities,DataAccess,Business ve tüm projelerde kullanılacak Core katmanı ve sunum katmanı için de WebAPI katmanı oluşturulur
Entities,DataAccess ve Business katmanlarına Concrete ve Abstract adında iki klasör oluşturulur.
Entities katmanında Concrete klasöründe Northwind veritabanı tablolarından Product,Category,Order ve Customer tabloları oluşturulur. Ayrıca Concrete klasörüne DTO ların classlarının tutulacağı DTOs adında bir klasör oluşturulur.
Core katmanında Entities,DataAccess ve Business katmanları oluşturulur.
Core katmanında Entities klasöründe IEntity adında bir interface oluşturulur.(public) ve DTO lar içinde IDto adında bir interface oluşturulur.
Entities katmanındaki veritabanı classlarına Core katmanında oluşturduğumuz IEntity referans olarak verilir.
------------------------
﻿using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete

{
    public class Product:IEntity
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string ProductName { get; set; }
        public short UnitsInStock { get; set; }
        public decimal UnitPrice { get; set; }

    }
}
---------------------------
Core katmanı altındaki DataAccess klasöründe IEntityRepository interface'i oluşturulur.
------------------------
﻿using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataAccess
{
    public interface IEntityRepository<T> where T : class,IEntity,new()
    {
        List<T> GetAll(Expression<Func<T,bool>>filter=null);
        T Get(Expression<Func<T, bool>> filter);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
       
    }
}
-------------------------
DataAccess katmanındaki Abstract klasöründe her bir veritabanı classı için interface oluşturulur.
------------------------
﻿using Core.DataAccess;
using Entities.Concrete;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstract
{
    public interface IProductDal:IEntityRepository<Product>
    {
        List<ProductDetailDto> GetProductDetails(); 
        

    }
}
--------------------------
Core katmanı DataAccess klasörü içinde EntityFramework adında bir klasör oluşturulur ve içine EfEntityRepositoryBase adında class oluşturulur.
Manage nuget package 'den entityframeworkcore eklenir
--------------------------
﻿using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataAccess.EntityFramework
{
    public class EfEntityRepositoryBase<TEntity,TContext>:IEntityRepository<TEntity>
        where TEntity : class,IEntity, new()
        where TContext:DbContext, new()
    {
        public void Add(TEntity entity)
        {
            using (TContext context = new TContext())
            {
                var addedEntity = context.Entry(entity);
                addedEntity.State = EntityState.Added;
                context.SaveChanges();
            }
        }
        public void Delete(TEntity entity)
        {
            using (TContext context = new TContext())
            {
                var deletedEntity = context.Entry(entity);
                deletedEntity.State = EntityState.Deleted;
                context.SaveChanges();
            }
        }
        public TEntity Get(Expression<Func<TEntity, bool>> filter)
        {
            using (TContext context = new TContext())
            {
                return context.Set<TEntity>().SingleOrDefault(filter);
            }
        }
        public List<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null)
        {
            using (TContext context = new TContext())
            {
                return filter == null ? context.Set<TEntity>().ToList() : context.Set<TEntity>().Where(filter).ToList();
            }
        }
        public void Update(TEntity entity)
        {
            using (TContext context = new TContext())
            {
                var updatedEntity = context.Entry(entity);
                updatedEntity.State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }
}
----------------------------
DataAccess katmanında Concrete klasörü içinde EntityFramework adında bir klasör açılır. Bu katmanda Context klasörü ve diğer veri tabanı dosyalarının class ları oluşturulur. Manage nuget package 'den entityframeworkcore eklenir
-------------------------
﻿using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework
{
    public class NorthwindContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=AYHAN;Database=NORTHWND;Trusted_Connection=true;TrustServerCertificate=true;");
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}
------------------------------
﻿using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfCategoryDal :EfEntityRepositoryBase<Category,NorthwindContext>, ICategoryDal
    {
        
    }
}
--------------------------
Business katmanındaki Abstract klasöründe her bir veritabanı dosyası için interface oluşturulur.
------------------------
﻿using Core.Utilities.Results;
using Entities.Concrete;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IProductService
    {
        IDataResult<List<Product>> GetAll();
        IDataResult<List<Product>> GetAllByCategoryId(int id);
        IDataResult<List<Product>> GetAllByUnitPrice(decimal min, decimal max);
        IDataResult<List<ProductDetailDto>> GetProductDetails();
        IDataResult<Product> GetById(int productId);
        IResult Add(Product product);
        IResult Update(Product product);
        IResult Delete(Product product);
    }
}
----------------------------
Business katmanında concrete klasöründe bu serviceler için manager dosyaları oluşturulur.
---------------------------
﻿using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class ProductManager : IProductService
    {
        IProductDal _productDal;
        public ProductManager(IProductDal productDal)
        {
            _productDal = productDal;
        }
        public IResult Add(Product product)
        {
            if (product.ProductName.Length < 2) 
            {
                return new ErrorResult(Messages.ProductNameInvalid);
            }
            _productDal.Add(product);
            return new SuccessResult(Messages.ProductAdded);
        }
        public IResult Update(Product product)
        {
            if(product.ProductName.Length < 2)
            {
                return new ErrorResult(Messages.ProductNameInvalid);
            }
            _productDal.Update(product);
            return new SuccessResult(Messages.ProductUpdated);
        }
        public IDataResult<List<Product>> GetAll()
        {
            if (DateTime.Now.Hour==22)
            {
                return new ErrorDataResult<List<Product>>(Messages.MaintenanceTime);
            }
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(),Messages.ProductsListed);
        }
        public IDataResult<List<Product>> GetAllByCategoryId(int id)
        {
            return new SuccessDataResult<List<Product>> (_productDal.GetAll(p=>p.CategoryId==id),Messages.ProductsListedWithCategory);
        }
        public IDataResult<List<Product>> GetAllByUnitPrice(decimal min, decimal max)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(p=>p.UnitPrice>=min && p.UnitPrice<=max),Messages.ProductsListedWithUnitPrice);
        }
        public IDataResult<Product> GetById(int productId)
        {
            return new SuccessDataResult<Product>(_productDal.Get   (p=>p.ProductId==productId),Messages.ProductDetailListed);
        }
        public IDataResult<List<ProductDetailDto>> GetProductDetails()
        {
            return new SuccessDataResult<List<ProductDetailDto>>(_productDal.GetProductDetails(),Messages.ProductsListed);
        }
        public IResult Delete(Product product)
        {
            _productDal.Delete(product);
            return new SuccessResult(Messages.ProductDeleted);
        }
    }
}
Burada oluşturduğumuz mesajları kontrol altına almak ve tek bir dosyadan değiştirmek için Business katmanında Constants adında bir klasör oluşturulur ve Messages adında public static bir class oluşturulur.
----------------------------
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Constants
{
    public static class Messages
    {
        public static string ProductAdded = "Ürün eklendi";
        public static string ProductNameInvalid = "Ürün ismi geçersiz";
        public static string MaintenanceTime = "Bakım zamanı";
        public static string ProductsListed = "Ürünler listelendi";
        public static string ProductsListedWithCategory = "İstediğiniz kategorideki ürünler listelendi";
        public static string ProductsListedWithUnitPrice = "İstediğiniz fiyat aralığındaki ürünler listelendi";
        public static string ProductDetailListed = "Detayını istediğiniz ürün";
        public static string ProductUpdated = "Ürün güncellendi";
        public static string ProductDeleted = "Ürün silindi";
        public static string CategoriesListed = "Kategoriler listelendi";
        public static string CategoryDetailListed = "Detayını istediğiniz kategori";
        public static string CategoryAdded = "Kategori eklendi";
        public static string CategoryUpdated = "Kategori güncellendi";
        public static string CategoryDeleted = "Kategori silindi";
        public static string CustomerAdded = "Müşteri eklendi";
        public static string CustomerDeleted = "Müşteri silindi";
        public static string CustomersListed = "Müşteriler listelendi";
        public static string CustomerDetailListed = "Detayını istediğiniz müşteri";
        public static string CustomerUpdated = "Müşteri güncellendi";
        public static string OrderAdded = "Sipariş eklendi";
        public static string OrderDeleted = "Sipariş silindi";
        public static string OrdersListed = "Siparişler listelendi";
        public static string OrdersDetailListed = "Detayını istediğiniz sipariş";
        public static string OrderUpdated = "Sipariş güncellendi.";
    }
}
-------------------------------
Manager lerde kullanılan IDataResult yapısı için Core katmanında Utilities klasörü ve içine Result adında bir klasör oluşturulur.
ve içinde IResult IDataRsult interfaceleri ,Result,DataResult,SuccessResult,SuccessDataResult,ErrorResult,ErrorDataResult classları oluşturulur.
-------------------------
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Results
{
    public interface IResult
    {
        bool IsSuccess { get; }
        string Message { get; }
    }
}
------------------------------
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Results
{
    public class Result : IResult
    {
        public Result(bool isSuccess, string message):this(isSuccess)
        {
            Message  = message;
        }
        public Result(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }
        public bool IsSuccess {  get; }
        public string Message { get; }
    }
}
----------------------------
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Results
{
    public class SuccessResult : Result
    {
        public SuccessResult(string message) : base(true,message)
        {
        }
        public SuccessResult() : base(true) 
        { 
        }
    }
}
----------------------------
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Results
{
    public class ErrorResult : Result
    {
        public ErrorResult(string message) : base(false, message)
        {
        }
        public ErrorResult() : base(false)
        {
        }
    }
}
------------------------------
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Results
{
    public interface IDataResult<T>:IResult  
    {
        T Data { get; }
    }
}
------------------------------
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Results
{
    public class DataResult<T> : Result, IDataResult<T>
    {
        public DataResult(T data,bool isSuccess,string message):base(isSuccess,message)
        {
            Data = data;            
        }
        public DataResult(T data,bool isSuccess):base(isSuccess)
        {
            Data = data;
        }  
        public T Data {  get;}
    }
}
-----------------------------
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Results
{
    public class SuccessDataResult<T> : DataResult<T>
    {
        public SuccessDataResult(T data,string message):base(data,true,message)
        {              
        }
        public SuccessDataResult(T data):base(data,true)
        {             
        }
        public SuccessDataResult(string message):base(default,true,message)
        {            
        }
        public SuccessDataResult():base(default,true)
        {            
        }        
    }
}
---------------------------------
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Results
{
    public class ErrorDataResult<T> : DataResult<T>
    {
        public ErrorDataResult(T data, string message) : base(data, false, message)
        {
        }
        public ErrorDataResult(T data) : base(data, false)
        {
        }
        public ErrorDataResult(string message) : base(default, false, message)
        {
        }
        public ErrorDataResult() : base(default, false)
        {
        }
    }
}
--------------------------
Business katmanında AOP ve inection işlemlerini yapmak için DependencyResolvers(Bağımlılık çözümleyiciler) adında bir klasör açılır ve içine Autofac klasörü açarız. ve içine AutofacBusinessModule adında bir class oluşturulur. Manage nuget package'den Autofac ve Autofac extras dynamic proxy eklenir
-------------------------
﻿using Autofac;
using Business.Abstract;
using Business.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DependencyResolvers.Autofac
{
    public class AutofacBusinessModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProductManager>().As<IProductService>().SingleInstance();
            builder.RegisterType<EfProductDal>().As<IProductDal>().SingleInstance();
            builder.RegisterType<CategoryManager>().As<ICategoryService>().SingleInstance();
            builder.RegisterType<EfCategoryDal>().As<ICategoryDal>().SingleInstance();
            builder.RegisterType<CustomerManager>().As<ICustomerService>().SingleInstance();
            builder.RegisterType<EfCustomerDal>().As<ICustomerDal>().SingleInstance();
            builder.RegisterType<OrderManager>().As<IOrderService>().SingleInstance();
            builder.RegisterType<EfOrderDal>().As<IOrderDal>().SingleInstance();
           
        }
    }
}
Bu injection işleminin çalışması için webAPI katmanındaki Program dosyasında şu değişiklik yapılır.
---------------------------
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Business.Abstract;
using Business.Concrete;
using Business.DependencyResolvers.Autofac;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//builder.Services.AddSingleton<IProductService,ProductManager>();
//builder.Services.AddSingleton<IProductDal,EfProductDal>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureContainer <ContainerBuilder>(builder =>
{
    builder.RegisterModule(new AutofacBusinessModule());
});

    
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
----------------------------
WebAPI clasöründe Controller klasöründe her bir veri tabanı için controller oluşturulur. Oluştururken tablo isminin çoğulu ve controller kullanılır.
-------------------------
using Business.Abstract;
using Business.Concrete;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var result=_productService.GetAll();
            if(result.IsSuccess )
            {
                return Ok(result);          
            }
            return BadRequest(result);
        }
        [HttpGet("getbyid")]
        public IActionResult GetById(int id)
        {
            var result = _productService.GetById(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost("add")]
        public IActionResult Add(Product product) 
        {
            var result=_productService.Add(product);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost("update")]
        public IActionResult Update(Product product) 
        {
            var result=_productService.Update(product);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost("delete")]
        public IActionResult Delete(Product product)
        {
            var result = _productService.Delete(product);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
-------------------------------
Şimdi validasyon işlemleri yapalım.Öncelikle Bussiness katmanına manage nuget package ile fluent validation eklenir. Business katmanına ValidationRules adında bir klasör açılır ve içine FluentValidation adında bir klasör daha açılır. Ve içine ProductValidator adında bir class oluşturulur.
----------------------------
﻿using Business.Constants;
using Entities.Concrete;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ValidationRules.FluentValidation
{
    public class ProductValidator : AbstractValidator<Product> 
    {
        public ProductValidator()
        {
            RuleFor(p => p.ProductName).NotEmpty().WithMessage(Messages.ProductNotEmpty);
            RuleFor(p => p.ProductName).MinimumLength(2).WithMessage(Messages.ProductNameMinTwoCharacter);
            RuleFor(p => p.UnitPrice).NotEmpty();
            RuleFor(p => p.UnitPrice).GreaterThan(0);
            RuleFor(p => p.UnitPrice).GreaterThanOrEqualTo(10).When(p => p.CategoryId == 1);
        }
    }
}
------------------------------
Bu validatoru kullanmak için bir tool oluşturuyoruz.Bu bir cross cutting concerns olayı olduğundan yani programı dikine kesen işlemler(Validation,cashing,Logging,Transaction vb.) olduğundan Core katmanında Cross Cutting Concerns adında bir klasör açılarak içine Validation adında bir klasör açılır ve içine ValidationTool adında bir class oluşturulur.
----------------------------
﻿using FluentValidation;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CrossCuttingConcerns.Validation
{
    public static class ValidationTool
    {
        public static void Validate(IValidator validator,object entity)
        {
            var context = new ValidationContext<object>(entity);
            var result = validator.Validate(context);
            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }
    }
}
---------------------------
Ancak biz validation işlemini aspect olarak uygulayacağız. Bu sebeple Core katmanında Utilities klasörü içinde Interceptors adında bir klasör oluşturuyoruz.
Bu klasör içine AspectInterceptorSelector,MethodInterception.MethodInterceptionBaseAttribute adında 3 tane class oluşturuyoruz.
------------------------------
﻿using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Interceptors
{
    public class AspectInterceptorSelector : IInterceptorSelector
    {
        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            var classAttributes = type.GetCustomAttributes<MethodInterceptionBaseAttribute>
                (true).ToList();
            var methodAttributes = type.GetMethod(method.Name)
                .GetCustomAttributes<MethodInterceptionBaseAttribute>(true);
            classAttributes.AddRange(methodAttributes);
            return classAttributes.OrderBy(x => x.Priority).ToArray();
        }
    }
}
--------------------------
﻿using Castle.DynamicProxy;

namespace Core.Utilities.Interceptors
{
    public abstract class MethodInterception : MethodInterceptionBaseAttribute
    {
        //invocation:business method
        protected virtual void OnBefore(IInvocation invocation) { }
        protected virtual void OnAfter(IInvocation invocation) { }
        protected virtual void OnException(IInvocation invocation, System.Exception e) { }
        protected virtual void OnSuccess(IInvocation invocation) { }
        public override void Intercept(IInvocation invocation)
        {
            var isSuccess = true;
            OnBefore(invocation);
            try
            {
                invocation.Proceed();
            }
            catch (Exception e)
            {
                isSuccess = false;
                OnException(invocation, e);
                throw;
            }
            finally
            {
                if (isSuccess)
                {
                    OnSuccess(invocation);
                }
            }
            OnAfter(invocation);
        }
    }
}
---------------------------
﻿using Castle.DynamicProxy;

namespace Core.Utilities.Interceptors
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public abstract class MethodInterceptionBaseAttribute : Attribute, IInterceptor
    {
        public int Priority { get; set; }
        public virtual void Intercept(IInvocation invocation)
        {
        }
    }
}
-----------------------------
Bunu Aspect olarak kullanacağımız için Core katmanı içinde Aspects adında bir klasör açılır ve içine önce Autofac adında bir klasör ve onun içine de Validation adında bir klasör daha açılır. ve içine ValidationAspect adında bir class açılır.
-------------------------
﻿using Castle.DynamicProxy;
using Core.CrossCuttingConcerns.Validation;
using Core.Utilities.Interceptors;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Aspects.Autofac.Validation
{
    public class ValidationAspect : MethodInterception
    {
        private Type _validatorType;
        public ValidationAspect(Type validatorType)
        {
            if (!typeof(IValidator).IsAssignableFrom(validatorType))
            {
                throw new System.Exception("Bu bir doğrulama sınıfı değil");
            }
            _validatorType = validatorType;
        }
        protected override void OnBefore(IInvocation invocation)
        {
            var validator = (IValidator)Activator.CreateInstance(_validatorType);
            var entityType = _validatorType.BaseType.GetGenericArguments()[0];
            var entities = invocation.Arguments.Where(t => t.GetType() == entityType);
            foreach (var entity in entities)
            {
                ValidationTool.Validate(validator, entity);
            }
        }
    }
}
--------------------------
Ama bunun uygulamaya kullanılacağını belirtmek gerekiyor bunu yapmak için Bussiness katmanında bulunan DependencyResolvers klasörü içindeki Autofac klasörü içindeki AutofacBusinessModule classına bir ekleme yapmak gerekiyor. Bu ekleme ile birlikte class şu şekile dönüşüyor.
-------------------------
﻿using Autofac;
using Autofac.Extras.DynamicProxy;
using Business.Abstract;
using Business.Concrete;
using Castle.DynamicProxy;
using Core.Utilities.Interceptors;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DependencyResolvers.Autofac
{
    public class AutofacBusinessModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProductManager>().As<IProductService>().SingleInstance();
            builder.RegisterType<EfProductDal>().As<IProductDal>().SingleInstance();
            builder.RegisterType<CategoryManager>().As<ICategoryService>().SingleInstance();
            builder.RegisterType<EfCategoryDal>().As<ICategoryDal>().SingleInstance();
            builder.RegisterType<CustomerManager>().As<ICustomerService>().SingleInstance();
            builder.RegisterType<EfCustomerDal>().As<ICustomerDal>().SingleInstance();
            builder.RegisterType<OrderManager>().As<IOrderService>().SingleInstance();
            builder.RegisterType<EfOrderDal>().As<IOrderDal>().SingleInstance();
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces()
                .EnableInterfaceInterceptors(new ProxyGenerationOptions()
                {
                    Selector = new AspectInterceptorSelector()
                }).SingleInstance();

        }
    }
}
---------------------------
Böylece Validasyon işlemi Aspect olarak kullanılabilir duruma geliyor. Bunu kullanmak için ProductManager de şu değişiklik yapılır.
--------------------------
﻿using Business.Abstract;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Validation;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class ProductManager : IProductService
    {
        IProductDal _productDal;
        public ProductManager(IProductDal productDal)
        {
            _productDal = productDal;
        }
        [ValidationAspect(typeof(ProductValidator))]
        public IResult Add(Product product)
        {
            _productDal.Add(product);
            return new SuccessResult(Messages.ProductAdded);
        }
        public IResult Update(Product product)
        {
            if(product.ProductName.Length < 2)
            {
                return new ErrorResult(Messages.ProductNameInvalid);
            }
            _productDal.Update(product);
            return new SuccessResult(Messages.ProductUpdated);
        }
        public IDataResult<List<Product>> GetAll()
        {
            if (DateTime.Now.Hour==22)
            {
                return new ErrorDataResult<List<Product>>(Messages.MaintenanceTime);
            }
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(),Messages.ProductsListed);
        }
        public IDataResult<List<Product>> GetAllByCategoryId(int id)
        {
            return new SuccessDataResult<List<Product>> (_productDal.GetAll(p=>p.CategoryId==id),Messages.ProductsListedWithCategory);
        }
        public IDataResult<List<Product>> GetAllByUnitPrice(decimal min, decimal max)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(p=>p.UnitPrice>=min && p.UnitPrice<=max),Messages.ProductsListedWithUnitPrice);
        }
        public IDataResult<Product> GetById(int productId)
        {
            return new SuccessDataResult<Product>(_productDal.Get   (p=>p.ProductId==productId),Messages.ProductDetailListed);
        }
        public IDataResult<List<ProductDetailDto>> GetProductDetails()
        {
            return new SuccessDataResult<List<ProductDetailDto>>(_productDal.GetProductDetails(),Messages.ProductsListed);
        }
        public IResult Delete(Product product)
        {
            _productDal.Delete(product);
            return new SuccessResult(Messages.ProductDeleted);
        }
    }
}
-------------------------
Add metodunda hemen üstüne validasyon yapılacağı ve bunun için ProductValidator'un kullanılacağı bildiriliyor.
İş kurallarını çalıştırmak için bir iş motoru yazıyoruz. Her ne kadar bu iş katmanını ilgilendirse de bu tüm diğer projelerde de kullanılacak bir yapı olduğundan bunu Core katmanı altında oluşturuyoruz. Bunun için Core katmanı Utilities klasörü içine Business adında bir klasör oluşturulup içine BusinessRules adında bir class oluşturuyoruz.
------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Core.Utilities.Results;

namespace Core.Utilities.Business
{
    public class BusinessRules
    {
        public static IResult Run(params IResult[] logics)
        {
            foreach (var logic in logics)
            {
                if (!logic.Success)
                {
                    return logic;
                }
            }
            return null;
        }
    }
}
------------------------
JWT sistemini kuruyoruz. Bu sistm kullanıcılara yetkilerine göre işlem yapabilme yetkisi ve sisteme giriş yetkisi vermeye yarar. SQL Server Obect Explorer (Wiew menüsünde) de Northwind database'ine 3 tane tablo ekliyoruz. Bu tablolar Users,OperationClaims ve UserOperationClaims Users tablosunda Id,FirstName,LastName,Email,PasswordHash,PasswordSalt,Status alanları bulunur. OperationClaims tablosunda Id,Name alanları, UserOperationClaims tablosunda Id,UserId,OperationClaimId alanları bulunacak. Daha sonra bu tabloların entities lerini oluşturuyoruz.
Bu tabloları Entities katmanında oluşturabiliriz. Ama biz bu yaptığımız yetkilendirmeyi tüm projelerde kullanacağımız bir yapı olacağından Core katmanı içinde yapıyoruz. Bu sebeple Core katmanındaki Entities klasörüne Concrete adında bir klasör açıyoruz. ve bu classları bunun içine oluşturuyoruz.
User class'ı
----------------------------
using Core.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Concrete
{
    public class User:IEntity
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool Status { get; set; }
    }
}
---------------------------
OperationClaim class'ı
-------------------------- 
using Core.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Concrete
{
    public class OperationClaim:IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

UserOperationClaim class'ı
----------------------------
using Core.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Concrete
{
    public class UserOperationClaim:IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OperationClaimId { get; set; }
    }
}

WebAPI katmanında appsetting dosyasında TokenOptions adında bir anahtar ekliyoruz.
---------------------
 "TokenOptions": {
    "Audience": "engin@engin.com",
    "Issuer": "engin@engin.com",
    "AccessTokenExpiration": 10,
    "SecurityKey": "mysupersecretkeymysupersecretkeymysupersecretkeymysupersecretkeymysupersecretkeymysupersecretkey"

  }, 
  Bu TokenOption içinde olmazsa olmaz bazı alanlar vardır ki bu token'in bize ait olduğu anlaşılsın. Bu alanlar Audience,Issuer,AccessTokenExpiration ve SecurityKey alanlarıdr.
  Audience kısmına sitemizin web adresini yada yukarıda görüldüğü gibi verebiliriz. Issuer alanına da aynı ismi vrebiliriz. AccessTokenExpiration oluşturduğumuz token'in geçerlilik süresini verir karşılığı dakika cinsindendir. SecurityKey alanı ise bu token'i kullanırken kullanacağımız anahtardır. Appsettings dosyasındaki bu düzenlemeyi yaptıktan sonra Core katmanındaki Utilities klasörüne Security adında bir klasör açıyoruz. Bu klasörün içindede Hashing,Encyription,JWT adında  3 tane alt klasör oluşturuyoruz.
  Hashing klasöründe HashingHelper adında bir class oluşturuyoruz.
  -------------------------
  ﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utilities.Security.Hashing
{
    public class HashingHelper
    {
        public static void CreatePasswordHash(string password,out byte[] passwordHash,out byte[] passwordSalt)
        {
            using (var hmac=new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
        public static bool VerifyPasswordHash(string password,byte[] passwordHash,byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i]!=passwordHash[i])
                    {
                        return false;
                    }
                }
                return true;
            }           
        }
    }
}
-------------------------
Şimdi Encryption klasöründe SecurityKeyHelper adında bir class oluşturuyoruz.
-------------------------
﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Core.Utilities.Security.Encryption
{
    public class SecurityKeyHelper
    {
        public static SecurityKey CreateSecurityKey(string securityKey)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
        }
    }
}
------------------------    
SecurityKey metodunu kullanabilmk için Nuget Package manager'dan Microsoft.Identity.ModelToken paketinin yüklenmesi gerekiyor. Visual studionun yeni sürümlerinde bunun üzrine geldiğinizde kolayca install edilebilmektedir. Eğer yüklüyse de using ile eklemeniz yapılabilmektedir. Bu klasöre birde SigningCredentialsHelper adında bir class daha oluşturuyoruz.
------------------------
﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Core.Utilities.Security.Encryption
{
    public class SigningCredentialsHelper
    {
        public static SigningCredentials CreateSigningCredentials(SecurityKey securityKey)
        {
            return new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha512Signature);
        }
    }
}
---------------------------
JWT klasöründe AccessToken adında bir class oluşturuyoruz.
--------------------------
﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utilities.Security.JWT
{
    public class AccessToken
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }

    }
}
-----------------------------
yine JWT klasöründe ITokenHelper isimli bir interface oluşturuyoruz.
----------------------------
﻿using System;
using System.Collections.Generic;
using System.Text;
using Core.Entities.Concrete;

namespace Core.Utilities.Security.JWT
{
    public interface ITokenHelper
    {
        AccessToken CreateToken(User user, List<OperationClaim> operationClaims);
    }
}
------------------------------
yine JWT klasöründe ITokenHelper'den inherite olan JWTHelper adında bir class oluşturuyoruz. Ama bunun için nuget package manager den System.IdendityModel.Tokns.jwt, Microsoft.Extensions.Configuration ve  Microsoft.Extensions.Configuration.Binder paketleri yüklenmelidir.
--------------------------
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Core.Entities.Concrete;
using Core.Utilities.Security.Encryption;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Core.Utilities.Security.JWT
{
    public class JwtHelper : ITokenHelper
    {
        public IConfiguration Configuration { get; }
        private TokenOptions _tokenOptions;
        private DateTime _accessTokenExpiration;
        public JwtHelper(IConfiguration configuration)
        {
            Configuration = configuration;
            _tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>();
        }
        public AccessToken CreateToken(User user, List<OperationClaim> operationClaims)
        {
            _accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration);
            var securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
            var signingCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);
            var jwt = CreateJwtSecurityToken(_tokenOptions, user, signingCredentials, operationClaims);
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.WriteToken(jwt);
            return new AccessToken
            {
                Token = token,
                Expiration = _accessTokenExpiration
            };
        }
        public JwtSecurityToken CreateJwtSecurityToken(TokenOptions tokenOptions, User user,
            SigningCredentials signingCredentials, List<OperationClaim> operationClaims)
        {
            var jwt = new JwtSecurityToken(
                issuer: tokenOptions.Issuer,
                audience: tokenOptions.Audience,
                expires: _accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: SetClaims(user, operationClaims),
                signingCredentials: signingCredentials
            );
            return jwt;
        }
       private IEnumerable<Claim> SetClaims(User user, List<OperationClaim> operationClaims)
 {
     var claims = new List<Claim>
     {
         new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
         new Claim(ClaimTypes.Email, user.Email),
         new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
     };
     claims.AddRange(operationClaims.Select(c => new Claim(ClaimTypes.Role, c.Name)).ToArray());
     return claims;
 }
    }
}

-------------------------
class taki IConfiguration bizim appsettings dosyamızı okumaya yarar. TokenOptions'un çalışabilmesi için bu JWT klasörü içine bu isimde bir class oluşturun. Bu bizim için bir Helper Class appsettings dosyasında TokenOptions için bir model bir veritabanı class'ı olmadığından ismide çoğul çünkü optionlar içeriyor.(Normalde veritabanı entity si oluştururken class ismi tekil veritabanında oluşan tablo ismi ise çoğul)
------------------------
﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utilities.Security.JWT
{
    public class TokenOptions
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int AccessTokenExpiration { get; set; }
        public string SecurityKey { get; set; }
    }
}
-------------------------
 private IEnumerable<Claim> SetClaims(User user, List<OperationClaim> operationClaims)
 {
     var claims = new List<Claim>();
     claims.AddNameIdentifier(user.Id.ToString());
     claims.AddEmail(user.Email);
     claims.AddName($"{user.FirstName} {user.LastName}");
     claims.AddRoles(operationClaims.Select(c => c.Name).ToArray());

     return claims;
 }
class'ındaki hatayı ortadan kaldırmak için class'ı extend ediyoruz. yani bu sınıfa kendi metotlarımızı ekliyoruz. Bu eklemeler AddNam,AddEmail,AddName,AddRoles metotlarıdır. Bir Extented class yazabilmek için hem class hemde metot static olmalıdır.
Bu sebeple Core katmanında Extensions adında bir klasör oluşturuyoruz ve içine ClaimExtensions adında bir static class oluşturuyoruz.
----------------------------
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Core.Extensions
{
    public static class ClaimExtensions
    {
        public static void AddEmail(this ICollection<Claim> claims, string email)
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, email));
        }
        public static void AddName(this ICollection<Claim> claims, string name)
        {
            claims.Add(new Claim(ClaimTypes.Name, name));
        }
        public static void AddNameIdentifier(this ICollection<Claim> claims, string nameIdentifier)
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, nameIdentifier));
        }
        public static void AddRoles(this ICollection<Claim> claims, string[] roles)
        {
            roles.ToList().ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role)));
        }
    }
}
--------------------------
Aynı şekilde extensions klasörüne ClaimsPrincipalExtensions adında yeni bir extend class oluşturuyoruz.
---------------------------
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Core.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static List<string> Claims(this ClaimsPrincipal claimsPrincipal, string claimType)
        {
            var result = claimsPrincipal?.FindAll(claimType)?.Select(x => x.Value).ToList();
            return result;
        }
        public static List<string> ClaimRoles(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal?.Claims(ClaimTypes.Role);
        }
    }
}
----------------------
Bu yetkilendirmeyi Aspect olarak yazacağız. yetkilendirme aspectleri genel olarak Business katmanına yazılır çünkü her projenin yetkilendirmesi farklıdır. bu sebeple tüm projelerde kullanacağımız Core katmanında yazılmaz. Bu sbeple Business katmanında BusinessAspects adında bir klasör oluşturuyoruz. İçine Autofac kullanacağımız için Autofac adında bir klasör oluşturuyoruz.Bu klasörün içine SecuredOperation adında bir class oluşturuyoruz.
------------------------
﻿using System;
using System.Collections.Generic;
using System.Text;
using Business.Constants;
using Castle.DynamicProxy;
using Core.Extensions;
using Core.Utilities.Interceptors;
using Core.Utilities.IoC;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Business.BusinessAspects.Autofac
{
    public class SecuredOperation:MethodInterception
    {
        private string[] _roles;
        private IHttpContextAccessor _httpContextAccessor;
        public SecuredOperation(string roles)
        {
            _roles = roles.Split(',');
            _httpContextAccessor =  ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
        }
        protected override void OnBefore(IInvocation invocation)
        {
            var roleClaims = _httpContextAccessor.HttpContext.User.ClaimRoles();
            foreach (var role in _roles)
            {
                if (roleClaims.Contains(role))
                {
                    return;
                }
            }
            throw new Exception(Messages.AuthorizationDenied);
        }
    }
}
------------------------
Bu class taki hataları çözmek için öncelikle nuget package manager'den yada hata veren metinin üzerine tıklayarak install komutu üzerinden using Microsoft.AspNetCore.Http paketini yüklüyoruz. Ayrıca ServiceTool classımızı da yazıyoruz. Bunu yazmak için Core katmanında Utilities klasöründe IoC adında yeni bir klasör açıyoruz ve içine ServiceTool adında bir class oluşturuyoruz.
-----------------------
﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Utilities.IoC
{
    public static class ServiceTool
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        public static IServiceCollection Create(IServiceCollection services)
        {
            ServiceProvider = services.BuildServiceProvider();
            return services;
        }
    }
}
-------------------------
Business katmanında bazı paketleri kurmamız gerekiyor. Bunlar Autofac,Autofac.Extensions.DependencyExtension ve Autofac.Extras.DynamicProxy bunlar kurulduktan sonra artık productManager sınıfında Add metodunda yetki kontrolü yaparken Add metodunun hemen üstünde  [SecuredOperation("product.add,Admin")] şeklinde kullanılabilir.
sistemimizi kurduk şimdi DataAccess katmanında Abstract klasöründe IUserDal interface'i oluşturuyoruz.
-------------------------
﻿using System;
using System.Collections.Generic;
using System.Text;
using Core.DataAccess;
using Core.Entities.Concrete;

namespace DataAccess.Abstract
{
    public interface IUserDal : IEntityRepository<User>
    {
        List<OperationClaim> GetClaims(User user);
    }
}
--------------------------
Ardından DataAccess katmanı Concrete klasöründe EfUserDal classını oluşturuyoruz. ve NorthwindContext classına da Jwt işlemi için oluşturduğumuz Core katmanındaki Entities klasöründeki Concrete klaösründeki 3 classımızı DbSet<> olarak ekliyoruz.
-------------------------
using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfUserDal : EfEntityRepositoryBase<User, NorthwindContext>, IUserDal
    {
        public List<OperationClaim> GetClaims(User user)
        {
            using (var context = new NorthwindContext())
            {
                var result = from operationClaim in context.OperationClaims
                             join userOperationClaim in context.UserOperationClaims
                                 on operationClaim.Id equals userOperationClaim.OperationClaimId
                             where userOperationClaim.UserId == user.Id
                             select new OperationClaim { Id = operationClaim.Id, Name = operationClaim.Name };
                return result.ToList();

            }
        }
    }
}
-------------------------
using Core.Entities.Concrete;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework
{
    public class NorthwindContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=AYHAN;Database=NORTHWND;Trusted_Connection=true;TrustServerCertificate=true;");
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OperationClaim> OperationClaims { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserOperationClaim> UserOperationClaims { get; set; }
    }
}
----------------------
Business katmanında Abstract klasöründe IUserService ve IAuthSrvice interface
----------------------
using Core.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IUserService
    {
        List<OperationClaim> GetClaims(User user);
        void Add(User user);
        User GetByMail(string email);
    }
}
-------------------------
﻿using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.JWT;
using Entities.DTOs;

namespace Business.Abstract
{
    public interface IAuthService
    {
        IDataResult<User> Register(UserForRegisterDto userForRegisterDto, string password);
        IDataResult<User> Login(UserForLoginDto userForLoginDto);
        IResult UserExists(string email);
        IDataResult<AccessToken> CreateAccessToken(User user);
    }
}
-----------------------

concrete klasöründ UserManager ve AuthManager classı
--------------------------
using Business.Abstract;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class UserManager : IUserService
    {
        IUserDal _userDal;
        public UserManager(IUserDal userDal)
        {
            _userDal = userDal;
        }
        public List<OperationClaim> GetClaims(User user)
        {
            return _userDal.GetClaims(user);
        }
        public void Add(User user)
        {
            _userDal.Add(user);
        }
        public User GetByMail(string email)
        {
            return _userDal.Get(u => u.Email == email);
        }
    }
}
----------------------
﻿using Business.Abstract;
using Business.Constants;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using Core.Utilities.Security.JWT;
using Entities.DTOs;

namespace Business.Concrete
{
    public class AuthManager : IAuthService
    {
        private IUserService _userService;
        private ITokenHelper _tokenHelper;
        public AuthManager(IUserService userService, ITokenHelper tokenHelper)
        {
            _userService = userService;
            _tokenHelper = tokenHelper;
        }
        public IDataResult<User> Register(UserForRegisterDto userForRegisterDto, string password)
        {
            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);
            var user = new User
            {
                Email = userForRegisterDto.Email,
                FirstName = userForRegisterDto.FirstName,
                LastName = userForRegisterDto.LastName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Status = true
            };
            _userService.Add(user);
            return new SuccessDataResult<User>(user, Messages.UserRegistered);
        }
        public IDataResult<User> Login(UserForLoginDto userForLoginDto)
        {
            var userToCheck = _userService.GetByMail(userForLoginDto.Email);
            if (userToCheck == null)
            {
                return new ErrorDataResult<User>(Messages.UserNotFound);
            }
            if (!HashingHelper.VerifyPasswordHash(userForLoginDto.Password, userToCheck.Data.PasswordHash, userToCheck.Data.PasswordSalt))
            {
                return new ErrorDataResult<User>(Messages.PasswordError);
            }
            return new SuccessDataResult<User>(userToCheck.Data, Messages.SuccessfulLogin);
        }
        public IResult UserExists(string email)
        {
            if (_userService.GetByMail(email) != null)
            {
                return new ErrorResult(Messages.UserAlreadyExists);
            }
            return new SuccessResult();
        }
        public IDataResult<AccessToken> CreateAccessToken(User user)
        {
            var claims = _userService.GetClaims(user);
            var accessToken = _tokenHelper.CreateToken(user, claims.Data);
            return new SuccessDataResult<AccessToken>(accessToken, Messages.AccessTokenCreated);
        }
    }
}
---------------------
Burada adı geçen iki Dto'yu da oluşturuyoruz bunu oluşturmak için Entities Katmanında Concrete klasöründe Dto klasöründe UserForLoginDto ve UserForRegisterDto adında 2 class oluşturuyoruz.
----------------------
﻿using System;
using System.Collections.Generic;
using System.Text;
using Core.Entities;

namespace Entities.DTOs
{
    public class UserForLoginDto : IDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
-----------------------
﻿using Core.Entities;

namespace Entities.DTOs
{
    public class UserForRegisterDto : IDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
-----------------------
ve şimdide WebAPI katmanında bunun controllerini yazıyoruz adı AuthController
-------------------------
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Abstract;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("login")]
        public ActionResult Login(UserForLoginDto userForLoginDto)
        {
            var userToLogin = _authService.Login(userForLoginDto);
            if (!userToLogin.Success)
            {
                return BadRequest(userToLogin.Message);
            }
            var result = _authService.CreateAccessToken(userToLogin.Data);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
        [HttpPost("register")]
        public ActionResult Register(UserForRegisterDto userForRegisterDto)
        {
            var userExists = _authService.UserExists(userForRegisterDto.Email);
            if (!userExists.Success)
            {
                return BadRequest(userExists.Message);
            }
            var registerResult = _authService.Register(userForRegisterDto, userForRegisterDto.Password);
            var result = _authService.CreateAccessToken(registerResult.Data);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }
    }
}
----------------------
Dpendency injection hatası almamak için Business katmanında DependencyInjections klasöründe Autofac klasöründe AutofacBusinessModule class'ına şu ifadeler ekleniyor.
----------------------
 builder.RegisterType<UserManager>().As<IUserService>();
 builder.RegisterType<EfUserDal>().As<IUserDal>();
 builder.RegisterType<AuthManager>().As<IAuthService>();
 builder.RegisterType<JwtHelper>().As<ITokenHelper>();
----------------------

WebAPI katmanında program class'ında şu eklemeler yapılır. Ama önce Microsoft.AspNetCore.Authentication.JwtBearer ve Microsoft.IdentityModel.Tokens paketleri yüklenir.
---------------------------
builder.Services.Configure<TokenOptions>(builder.Configuration.GetSection("TokenOptions"));
var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<TokenOptions>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = tokenOptions.Issuer,
            ValidAudience = tokenOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey)
        };
    });
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureContainer <ContainerBuilder>(builder =>
{
    builder.RegisterModule(new AutofacBusinessModule());
});

en altta app. ile başlayan yere
app.UseAuthentication();
ekle

------------------------

Authmanager de userexist metodunda hata olduğundan yeni kullanıcı register olamıyordu onu şu şekilde değiştiriyoruz.
-----------------------
  public IResult UserExists(string email)
  {
      var result = _userService.GetByMail(email);
      if (result.Data!=null)
      {
          return new ErrorResult(Messages.UserAlreadyExists);
      }
      return new SuccessResult();
  }
  -------------------------
Genel bağımlılıkları yani tüm projelerde kullanacağımız bağımlılıkları (dependency injection) yapmak için Core katmanında Utilities klasöründe IoC klasörüne bir ICoreModule adında bir interface oluşturuyoruz. Aynı business katmanına oluşturduğumuz gibi buradada DependencyResolvers adında bir klasör oluşturuyoruz ve içine CoreModule adında bir class oluşturuyoruz.
------------------------
 using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.IoC
{
    public interface ICoreModule
    {
        void Load(IServiceCollection serviceCollection);     
    }
}

  ---------------------------
using Core.Utilities.IoC;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DependencyResolvers
{
    public class CoreModule : ICoreModule
    {
        public void Load(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }
    }
}
-----------------------------
Buradaki hatanın ortadan kalkması için Microsoft.AspNetCore.Http paketinin kurulması gerekiyor
program.cs dosyasında
builder.Services.AddHttpContextAccessor();
yerine hertürlü dependencyinjectionları ekleyeceğim ve içine sadece coremodule değil her tür bağımlılığı ekleyeceğim bir yapı kurmak istiyorum bunun için Core katmanında Extensions klasöründe ServiceCollectionExtensions adında bir class oluşturuyoruz. 
------------------------
using Core.Utilities.IoC;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDependencyResolvers(this IServiceCollection serviceCollection, ICoreModule[] modules) 
        {
            foreach (var module in modules)
            { 
            module.Load(serviceCollection);
            }
            return ServiceTool.Create(serviceCollection);

        }

    }
}
----------------------------
Bu Extension'u program.cs içine ekliyoruz.
--------------------------
builder.Services.AddDependencyResolvers(new ICoreModule[]
{
    new CoreModule()
});
-------------------------
Bu yapıyı zaten çalışan bir bağımlılık yapısını dahada profesyonel hale getirmek için buraya eklenen her modulü çalıştaracak bir yapı şeklinde oluşturduk
  Bu arada SQL Server Explorer'den users tablosundaki binary500 alanlarını varbinary(500) olarak değiştirerek update ediyoruz.
  Şimdi bir Cachleme sistemi yapalım. Yani eğer herhangi bir ekleme ve güncelleme işlemi yapılmadıysa yani veritabanındaki tablolarda herhangi bir değişiklik olmadıysa çağrılan veri tabanı verilerinin veritabanına gidilmeden cache(memory) den hızlı olarak getirilmesidir. Eğer veritabanını değiştiren bir işlem yapıldıysa ekleme,güncelleme,silme bu kezde cache'den değil veritabanına gidilmesini sağlayacağız. Cachlenmek istenen veri key,value pair dediğimiz kavramlarla tutuyoruz. key dediğimiz cach'e verilen isim
  Buna Aspect yazmadan önce Core katmanında öncelikle alt yapısını oluşturuyoruz. Bunun için öncelikle Cross Cutting Concerns klasörüne Caching adlı bir klasör oluşturuyoruz. Biz cache işlemi için AspNet'in inmemory yöntemini kullanacağız. Caching klasörü içinde öncelikle ICachManager adında bir interface oluşturulur.
  ------------------------
  using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CrossCuttingConcerns.Caching
{
    public interface ICacheManager
    {
        T Get<T>(string key);
        object Get(string key);
        void Add(string key, object value, int duration);
        bool IsAdd(string key);
        void Remove(string key);
        void RemoveByPattern(string pattern);

    }
}
------------------------
şimdi bu interface'in implementasyonunu yapıyoruz. Caching klasörü içine Microsoft adında bir klasör açıyoruz.ve içine MemoryCachManager adında bir class oluşturuyoruz. 
-----------------------
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Core.Utilities.IoC;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Core.CrossCuttingConcerns.Caching.Microsoft
{
    public class MemoryCacheManager:ICacheManager
    {
        //Adapter Pattern
        IMemoryCache _memoryCache;
        public MemoryCacheManager()
        {
            _memoryCache = ServiceTool.ServiceProvider.GetService<IMemoryCache>();
        }
        public T Get<T>(string key)
        {
            return _memoryCache.Get<T>(key);
        }
        public object Get(string key)
        {
            return _memoryCache.Get(key);
        }
        public void Add(string key, object value, int duration)
        {
            _memoryCache.Set(key, value, TimeSpan.FromMinutes(duration));
        }
        public bool IsAdd(string key)
        {
            return _memoryCache.TryGetValue(key,out _);
        }
        public void Remove(string key)
        {
           _memoryCache.Remove(key);
        }
        public void RemoveByPattern(string pattern)
        {
            var cacheEntriesCollectionDefinition = typeof(MemoryCache).GetProperty("EntriesCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var cacheEntriesCollection = cacheEntriesCollectionDefinition.GetValue(_memoryCache) as dynamic;
            List<ICacheEntry> cacheCollectionValues = new List<ICacheEntry>();
            foreach (var cacheItem in cacheEntriesCollection)
            {
                ICacheEntry cacheItemValue = cacheItem.GetType().GetProperty("Value").GetValue(cacheItem, null);
                cacheCollectionValues.Add(cacheItemValue);
            }
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var keysToRemove = cacheCollectionValues.Where(d => regex.IsMatch(d.Key.ToString())).Select(d => d.Key).ToList();
            foreach (var key in keysToRemove)
            {
                _memoryCache.Remove(key);
            }
        }
    }
}
Bunu yaptıktan sonra Core katmanında DependencyResolvers klasörü içinde oluşturmuş olduğumuz CoreModule class'ına şu eklemeyi yapıyoruz.
----------------------
 serviceCollection.AddMemoryCache();
  serviceCollection.AddSingleton<ICacheManager, MemoryCacheManager>();
  -----------------------
  CoreModule class'ı şu şekilde olacak
  ------------------------
  ﻿using System;
using System.Collections.Generic;
using System.Text;
using Core.CrossCuttingConcerns.Caching;
using Core.CrossCuttingConcerns.Caching.Microsoft;
using Core.Utilities.IoC;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Core.DependencyResolvers
{
    public class CoreModule:ICoreModule
    {
        public void Load(IServiceCollection serviceCollection)
        {
            serviceCollection.AddMemoryCache();
            serviceCollection.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            serviceCollection.AddSingleton<ICacheManager, MemoryCacheManager>();
        }
    }
}
--------------------------
Şimdi bunun aspectini yazıp cache işlemini bitiriyoruz. Core katmanında Aspect Klasöründe Caching adında bir klasör oluşturup içine CacheAspect isminde bir class oluşturuyoruz.
---------------------
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;
using Core.CrossCuttingConcerns.Caching;
using Core.Utilities.Interceptors;
using Core.Utilities.IoC;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Aspects.Autofac.Caching
{
    public class CacheAspect : MethodInterception
    {
        private int _duration;
        private ICacheManager _cacheManager;
        public CacheAspect(int duration = 60)
        {
            _duration = duration;
            _cacheManager = ServiceTool.ServiceProvider.GetService<ICacheManager>();
        }
        public override void Intercept(IInvocation invocation)
        {
            var methodName = string.Format($"{invocation.Method.ReflectedType.FullName}.{invocation.Method.Name}");
            var arguments = invocation.Arguments.ToList();
            var key = $"{methodName}({string.Join(",", arguments.Select(x => x?.ToString() ?? "<Null>"))})";
            if (_cacheManager.IsAdd(key))
            {
                invocation.ReturnValue = _cacheManager.Get(key);
                return;
            }
            invocation.Proceed();
            _cacheManager.Add(key, invocation.ReturnValue, _duration);
        }
    }
}
-----------------------
Yine aynı klasör içine  isminde bir class daha oluşturuyoruz.
----------------------
﻿using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;
using Core.CrossCuttingConcerns.Caching;
using Core.Utilities.Interceptors;
using Core.Utilities.IoC;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Aspects.Autofac.Caching
{
    public class CacheRemoveAspect : MethodInterception
    {
        private string _pattern;
        private ICacheManager _cacheManager;
        public CacheRemoveAspect(string pattern)
        {
            _pattern = pattern;
            _cacheManager = ServiceTool.ServiceProvider.GetService<ICacheManager>();
        }
        protected override void OnSuccess(IInvocation invocation)
        {
            _cacheManager.RemoveByPattern(_pattern);
        }
    }
}
 -------------------------

 Böylece cachleme işlemi tamamlanmış oldu bunu productManager'de şu şekilde kullanıyoruz add,update ve delete CacheRemoveAspect kullanılırken listeleme işlemleri için cacheAspect kullanılır. CacheRemoveAspect uygulanırken o servisin(Burada productservice) get key'i silinir böylece eğer bir ürün ekleme,güncelleme,silme işlemleri yapıldığında listeleme işlemi cach den değil veri tabanından getirilir. şu şekilde kullanılır.
 --------------------------
  [SecuredOperation("product.add,Admin")]
 [ValidationAspect(typeof(ProductValidator))]
 [CacheRemoveAspect("IProductService.Get")]
 public IResult Add(Product product)
 {
    var Toplam= _productDal.GetAll(p => p.CategoryId == product.CategoryId).Count;
     if (Toplam >= 15)
     {
         return new ErrorResult();
     }    
     _productDal.Add(product);
     return new SuccessResult(Messages.ProductAdded);
 }
 --------------------------
   [CacheAspect]
  public IDataResult<List<Product>> GetAll()
  {
      if (DateTime.Now.Hour==22)
      {
          return new ErrorDataResult<List<Product>>(Messages.MaintenanceTime);
      }
      return new SuccessDataResult<List<Product>>(_productDal.GetAll(),Messages.ProductsListed);
  }
  ------------------------
  Şimdi transaction işlemi yapacağız. Bunun ne anlama geldiğiniz şu örnekle açıklayabiliriz. Örneğin bir para aktarım işlemi düşünelim A kişisi bir B kişisine para aktarma işlemi yaparken diyelimki 100 TL gönderecek gönderme işlemi sırasında A kişisinin hesabından 100 TL azalacak şekilde update yapılırken B kişisinin hesabıda 100 TL artacak şekilde update yapılır diyelimki A kişisinin hesabımdan 100 TL çekme işlemi başarılı oldu ama B kişisinin hesabına aktarılırken bir hata oluştu ve hesabı 100 TL artmadı. işte böyle bir durumda yapılan işlemin geri alınarak A kişisinin 100 TL si geri hesaba iade edilmelidir. işte bu işlem Transaction işlemidir. şimdi bu sistemi kuralım.
  Bunun için Core katmanında Aspects klasöründe Autofac klasöründe Transaction adında bir klasör açılır ve içine TransactionScopeAspect adında bir class oluşturulur.
  ------------------------
  ﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using Castle.DynamicProxy;
using Core.Utilities.Interceptors;

namespace Core.Aspects.Autofac.Transaction
{
    public class TransactionScopeAspect : MethodInterception
    {
        public override void Intercept(IInvocation invocation)
        {
            using (TransactionScope transactionScope = new TransactionScope())
            {
                try
                {
                    invocation.Proceed();
                    transactionScope.Complete();
                }
                catch (System.Exception e)
                {
                    transactionScope.Dispose();
                    throw;
                }
            }
        }
    }
}
---------------------------
IProductService te transaction şu şekilde ekleme yapıyoruz.
---------------------------
  IResult TransactionalOperation(Product product);
  -------------------------
  ProductManager'e implemente ediyoruz.
  -------------------------
    [TransactionScopeAspect]
        public IResult TransactionalOperation(Product product)
        {
            _productDal.Update(product);
            _productDal.Add(product);
            return new SuccessResult(Messages.ProductUpdated);
        }
        -----------------------
Controller'de şu eklemeyi yapıyoruz.
---------------------
  [HttpPost("transaction")]
        public IActionResult TransactionTest(Product product)
        {
            var result = _productService.TransactionalOperation(product);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }
        ------------------------
şimdi de performans aspect işlemini yapalım bunun için öncelikle core katmanında dependencyinjections klasöründe coremodule classına şu eklemeyi yapıyoruz.
-------------------------
ş           serviceCollection.AddSingleton<Stopwatch>();
-------------------------
Core katmanına Aspects klasöründe Autofac klasörüne performance klasörü oluşturuyoruz ve içine PerformanceAspect adında bir class oluşturuyoruz.
------------------------
﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Castle.DynamicProxy;
using Core.Utilities.Interceptors;
using Core.Utilities.IoC;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Aspects.Autofac.Performance
{
    public class PerformanceAspect:MethodInterception
    {
        private int _interval;
        private Stopwatch _stopwatch;
        public PerformanceAspect(int interval)
        {
            _interval = interval;
            _stopwatch = ServiceTool.ServiceProvider.GetService<Stopwatch>();
        }
        protected override void OnBefore(IInvocation invocation)
        {
            _stopwatch.Start();
        }
        protected override void OnAfter(IInvocation invocation)
        {
            if (_stopwatch.Elapsed.TotalSeconds>_interval)
            {
                Debug.WriteLine($"Performance : {invocation.Method.DeclaringType.FullName}.{invocation.Method.Name}-->{_stopwatch.Elapsed.TotalSeconds}");
            }
            _stopwatch.Reset();
        }
    }
}
----------------------
Kullanımı için kontrol edilmesini istediğiniz metodun üstüne [PerformanceAspect(5)] gibi bir kullanım yaparsanız bu şu demek bu metodun yüklenişi 5 saniyeyi aşarsa bana haber ver. Eğer bunu tüm metotları kontrol edecek şekilde Core katmanında Utilities klasöründe interceptors klasöründe AspectInterceptorSelector classında eklerseniz bütün mtotlarda prformans kontrolü yapar.
--------------------------
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Castle.DynamicProxy;
using Core.Aspects.Autofac.Exception;
using Core.CrossCuttingConcerns.Logging.Log4Net.Loggers;

namespace Core.Utilities.Interceptors
{
    public class AspectInterceptorSelector:IInterceptorSelector
    {
        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            var classAttributes = type.GetCustomAttributes<MethodInterceptionBaseAttribute>
                (true).ToList();
            var methodAttributes = type.GetMethod(method.Name)
                .GetCustomAttributes<MethodInterceptionBaseAttribute>(true);
            classAttributes.AddRange(methodAttributes);
            classAttributes.Add(new ExceptionLogAspect(typeof(FileLogger)));
            return classAttributes.OrderBy(x => x.Priority).ToArray();
        }
    }
}
------------------------
burada log işlemi eklenmiş aynı şekilde performanceAspect eklenecek.
--------------------------
Diğer aspectleri eklemeyi şimdilik bırakalım ve işin Frontend kısmına biraz bakalım.
ANGULAR
-----------------
Anguları kurmak için öncelikle nodejs i kurmamız gerekir. nodejs.org sayfasında nodejs'i donload ediyor ve kuruyoruz. Ayrıca angular komutlarını kullanmak için ve uygulamayı yazmak için visual code 'u da kuruyoruz. Daha sonra komut istem ekranını(cmd,Powershell vb.) açıyoruz. Yada visual studio code'u açarak terminal çalıştırıyoruz. 
npm install -g @angular/cli komutuyla angular paketlerini yüklüyoruz daha sonra angular projelerini oluşturmak için bir klasör oluşturuyoruz ve komut satırımızı o klasörün içine girecek şekilde düzenliyoruz
C:\Users\ayhan\source\Projelerin Angular Hali>
buradayken ng new Northwind komutuyla northwind adında angular projemizi oluşturuyoruz. cd northwind yazarak oluşturduğumuz projenin içine giriyoruz ve code . ile visual code'u bu proje dosyalarıyla beraber açıyoruz. Açılan visual code programında sol tarafta yüklü olan dosyalar görülür. package.json dosyasında Dependencies kısmında yüklü olan paketler görülür. Burada çok önemli olan kısımlardan biri node module kısmıdır. Burada angularda kullanılan paketler mevcuttur ancak kullanmak için kurmak gerekir. Ve angularda yaptığımız değişikliklerden sonra gituba gönderdiğimizde gönderilen dosyaların içinde node module kısmı bulunmaz. Bu sebeple eğer biz gitub daki bir repositoryi angulara alıp çalıştırmak istersek yapacağımız ilk şey ilgili klasör içindeyken npm install ile gerekli olan paketleri yüklemek olmalıdır. Bir diğer çok önemli klasör de src klasörüdür. src source kelimesinden gelir yani kaynak anlamına gelir ve bizim proenin bütün kaynak kodları bu klasör içinde bulunur. src klasörünün içinde bulunan app klasöründe uygulama kodları bulunur.    src klasörü içinde index.htm adında bir dosya bulunur bu sayfada bulunan  <app-root></app-root> bir komponenttir. Biz angularda componentler oluşturarak sayfaları oluşturacağız. app klasörü içine components adında bir klasör açıyoruz.  <app-root></app-root> burada görülen bu component app.component.ts'den gelir. Buradaki uzantılarına göre app.component.css app componenti ilgilendiren sayfanın tasarımını içerir. app.component.html app component sayfasını gösterir.app.component.spec.ts componentin unit test sayfasıdır. bizim için çok önemli iki dosya bir sadece ts uzantılıolan dosya ve html uzantılı olan dosyadır.    Angularda biz yazmış olduğumuz backend'i yöneterek dataların işlenmesini sağlayacağız. bunun için kullanacağımız componentlerin ts sayfasıdır. Yeni bri component oluşturmak için components klasörüne sağ tıklıyoruz ve orada open in integrated terminal'e tıklıyoruz bunu yapınca komut satırımız bunun içine girecek şekilde düzenlenir.
ng g component product komutu ile product componenti oluşturulur. aynı şekilde category ve navi componentlerini de oluşturuyoruz. navi componenti sayfanın üstünde bulunan menü çubuğu
Biz bir component oluşturduğumuzda appmodule dosyasında eklenir. Ancak Anguların yeni versiyonlarında appmodule dosyası oluşmaz. eğer oluşmasını istersek yeni proje oluşturuken
 ng new --no-standalone projeadı komutu ile proje oluştururuz. Böylece appmodule oluşur. Componentlerimizi oluşturduktan sonra appmodule dosyasına şu şekilde ekleme yapılır.
 -------------------------------
 import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ProductComponent } from './components/product/product.component';
import { CategoryComponent } from './components/category/category.component';
import { NaviComponent } from './components/navi/navi.component';

@NgModule({
  declarations: [
    AppComponent,
    ProductComponent,
    CategoryComponent,
    NaviComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
-------------------------------
oluşturulan component appmodule dosyasına importu ve declaration'u eklenir. 
Bizim birde index.html dosymız var bu index.html dosyasında component hiyerarşisini kuruyoruz.appcomponent.html ilk açılan html sayfasıdır.bu html sayfasında diğer componentleri göstermek istersek şu şekilde componentlerin selectorlerini ekleriz.
---------------------------------
<!-- Welcome to {{title}}
Hello {{user}}
<ul>
  <li *ngFor="let product of products">{{product.productName}}</li>
</ul> -->
<app-navi></app-navi>
<app-category></app-category>
<app-product></app-product>
<router-outlet><router-outlet>
----------------------------------
Şimdi üste bir navbar oluşturalım bunun için getbootstrap sayfasına gidip oradan docs kısmında components ler kısmında navbar bölümünden istediğimiz navbar'ın kodlarını kopyalıyoruz ve navi componentine gidip navi.component.html sayfasındakileri silip kopyaladığımız bu kodları yapıştırıyoruz.
----------------------------------
<nav class="navbar navbar-expand-lg bg-body-tertiary">
  <div class="container-fluid">
    <a class="navbar-brand" href="#">Navbar</a>
    <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
      <span class="navbar-toggler-icon"></span>
    </button>
    <div class="collapse navbar-collapse" id="navbarSupportedContent">
      <ul class="navbar-nav me-auto mb-2 mb-lg-0">
        <li class="nav-item">
          <a class="nav-link active" aria-current="page" href="#">Home</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="#">Link</a>
        </li>
        <li class="nav-item dropdown">
          <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown" aria-expanded="false">
            Dropdown
          </a>
          <ul class="dropdown-menu">
            <li><a class="dropdown-item" href="#">Action</a></li>
            <li><a class="dropdown-item" href="#">Another action</a></li>
            <li><hr class="dropdown-divider"></li>
            <li><a class="dropdown-item" href="#">Something else here</a></li>
          </ul>
        </li>
        <li class="nav-item">
          <a class="nav-link disabled" aria-disabled="true">Disabled</a>
        </li>
      </ul>
      <form class="d-flex" role="search">
        <input class="form-control me-2" type="search" placeholder="Search" aria-label="Search">
        <button class="btn btn-outline-success" type="submit">Search</button>
      </form>
    </div>
  </div>
</nav>
-------------------------
Ancak bunu yapıştırdığımızda sayfamız düzgün olarak görülmez.Bunun nedeni bu sayfada bulunan bootstrap css'lerinin devreye girmemesidir. Bunun için bootstrap'in install edilmesi gerekir.
npm i bootstrap@5.3.3
Bunu kurduktan sonra anguların configürasyon dosyası olan angular.json dosyasında styles kısmına yalnız test için olana değil ilk kısımdaki styles bölümüne
"node_module/bootstrap/dist/bootstrap.min.css",
bu satırı ekleyince görüntü düzeliyor. Ancak sağ v sol taraflarda neredeyse hiç boşluk yok bunu düzeltmek app.component.html dosyasındaki herşeyi bir container içine alalım.
-------------------------

<div class="container" >
  <app-navi></app-navi>
  <app-category></app-category>
  <app-product></app-product>

</div>
<router-outlet><router-outlet>
----------------------------
eski versiyonlarda <router-outlet><router-outlet> da <div> </div> in içine konuyordu ancak yeni versiyonlarda içine koyunca hata veriyor.
şimdi biz şöyle bir tasarım yapmak istiyoruz. klasik e-ticaret sitelerinde olduğu gibi sol tarafta kategoriler sütunu ve ortada ürünler bulunsun. Bunun için biz satır kullanacağız 
-----------------------------
<div class="container">
  <app-navi></app-navi>
  <div class="row">
    <div class="col-md-3">
      <app-category></app-category>
    </div>
    <div class="col-md-9">
      <app-product></app-product>
    </div>
  </div>
</div>
<router-outlet><router-outlet> 
--------------------------
Bunu yapınca kategoriler solda 3 birimlik ürünler ise onun yanında 9 birimlik(Sayfa toplam 12 birime ayrılır.) şekilde aynı satıra yerleşir. Şimdi de sol tarafta kategorileri gösterelim.
Bootstrap sayfasına gidip compenents ler kısmından bir adet list group kodu alıp bu kodu category.html sayfasına yapıştırıyoruz. bunu yapınca sol tarafta kategorileri listeleyeceğimiz list-group eklenmiş oluyor.
-------------------------
<ul class="list-group">
  <li class="list-group-item">An item</li>
  <li class="list-group-item">A second item</li>
  <li class="list-group-item">A third item</li>
  <li class="list-group-item">A fourth item</li>
  <li class="list-group-item">And a fifth one</li  
</ul>
-----------------------------
şimdi bizim apiden gelen veriyi karşılayacak yapıyı oluşturalım ve veriler veritabanından gelsin. app kısmında sağ tıklayarak yeni bir klasör oluşturuyoruz ve adını models koyuyoruz. ve içine product.ts dosyası oluşturuyoruz.
c# da public terimi yerine burada export kullanıyoruz.
------------------------------
    export interface Product {
        productId:number
        categoryId:number
        productName:string
        unitsInStock:number
        unitPrice:number
    }
    ----------------------------
    product componentimizdeki app.product.ts dosyamızda şu değişikliği yapıyoruz.
    ---------------------------------
    import { Component } from '@angular/core';
import { Product } from '../../models/product';

@Component({
  selector: 'app-product',
  standalone: false,

  templateUrl: './product.component.html',
  styleUrl: './product.component.css',
})
export class ProductComponent {
  products: Product[] = [
   
  ];
}
-------------------------------
models klasöründe productResponseModel.ts adında bir dosya oluşturuyoruz. Burada bana gelen datayı karşılayacak bir model oluşturuyoruz.
------------------------------
import { Product } from "./product";

export interface ProductResponseModel{
    data:Product[]
    isSuccess:boolean
    message:string
}
----------------------------------
models klasörü içine bir de responseModel.ts oluşturuyoruz çünkü herseferinde isSuccess ve mesaj bilgisini göstermek istemeyebiliriz. bu sebeple yukarıdaki ProductResponseModel.ts dosyasındaki isSuccess:boolean ve message:string kısmını responseModel.ts dosyasına alıyoruz. (keserek alıyoruz.)
-------------------------------
export interface ResponseModel{
    isSuccess:boolean
    message:string
}
---------------------------
ama tabi ProductResponseModel extend ediyoruz. yani şu şekle dönüşüyor.
-----------------------------
import { Product } from "./product";
import { ResponseModel } from "./responseModel";

export interface ProductResponseModel extends ResponseModel{
    data:Product[]
  
}
---------------------------
şimdi product.component.ts dosyasında apiden gelen veriyi karşılamaya çalışıyoruz. apiye bağlanmak için httpClient nesnesini kullanıyoruz .Bunu kullanmak için  product.component.ts dosyasına import { HttpClient } from '@angular/common/http';
ekliyoruz ve constructor içinde constructor(private httpClient:HttpClient){}
tanımlamasını eklemeliyiz.
--------------------------------
import { Component, OnInit } from '@angular/core';
import { Product } from '../../models/product';
import { ProductResponseModel } from '../../models/productResponseModel';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-product',
  standalone: false,

  templateUrl: './product.component.html',
  styleUrl: './product.component.css',
})
export class ProductComponent implements OnInit {
  products: Product[] = [];
  apiUrl = 'https://localhost:7206/api/products/getall';
 
  constructor(private httpClient: HttpClient) {}

  ngOnInit(): void {
   this.getProducts()
  }

  getProducts() {
    this.httpClient
      .get<ProductResponseModel>(this.apiUrl)
      .subscribe((response) => {
        this.products=response.data
      });
  }
}
----------------------------
şimdi product.component.html dosyasında eksik olan unitsInStock ekleniyor
--------------------------
<table class="table">
    <tr>
        <th>Ürün Id</th>
        <th>Kategori Id</th>
        <th>Ürün Adı</th>
        <th>Fiyatı</th>
    </tr>
    <tr *ngFor="let product of products">
        <td>{{product.productId}}</td>  
        <td>{{product.categoryId}}</td>
        <td>{{product.productName}}</td>
        <td>{{product.unitPrice}}</td>
        <td>{{product.unitsInStock}}</td>

    </tr>
</table>
--------------------------
Bunu yaptıktan sonra app.module.ts dosyasında provider kısmına provideHttpClient() ekleniyor. Daha önce HttpClientModule imports kısmına ekleniyor ve import ediliyordu ancak anguların yeni versiyonlarında bu kod kaldırılmış yerine provideHttpClient() gelmiştir.
---------------------------
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ProductComponent } from './components/product/product.component';
import { CategoryComponent } from './components/category/category.component';
import { NaviComponent } from './components/navi/navi.component';
import { provideHttpClient } from '@angular/common/http';

@NgModule({
  declarations: [
    AppComponent,
    ProductComponent,
    CategoryComponent,
    NaviComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
  ],
  providers: [
    provideHttpClient()
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
-----------------------------
Bu değişiklikten sonra injection hatası kalkmış ancak XMLHttpRquest hatası vermiştir.Yani CORS hatası vermiştir.Backende gelen isteğin güvenilir bir adresten geldiğini backende bildirmemiz gerekiyor bunun için backend'de webAPI kısmında program.cs dosyasında builder.Services.AddCors(); eklemesi yapıyoruz. buradaki ekleme sırası önemli değil ama builder.Services.AddControllers(); sonrasına eklenebilir ama configuration kısmında app.UseHttpsRedirection(); kısmından öncesinde app.UseCors(builder=>builder.WithOrigins("https://localhost:4200","http://localhost:4200").AllowAnyHeader());
eklemesini yapıyoruz.
----------------------------
Biz bunu servis ekleyerek oluşturacağız. app katmanında sağ tıklıyor ve services adında bir klasör oluşturuyoruz. oluşturduğumuz services klasörüne sağ tıklıyoruz ve open integrated terminali seçerek o kısma giriyoruz ve ng g service product ile product servisini oluşturuyoruz. componentteki product.ts dosyasından bazı verileri alarak product.service.ts dosyasına geçiriyoruz. ve product.service.ts dosyası şu şekilde oluyor.
---------------------------
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ProductResponseModel } from '../models/productResponseModel';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  apiUrl = 'https://localhost:7206/api/products/getall';


  constructor(private httpClient: HttpClient) { }

   getProducts():Observable<ProductResponseModel> {
     return this.httpClient.get<ProductResponseModel>(this.apiUrl);
       
        }
    }

-------------------------
app.product.component.ts dosyası şu şekilde oluyor.
-------------------------
import { Component, OnInit } from '@angular/core';
import { Product } from '../../models/product';
import { ProductResponseModel } from '../../models/productResponseModel';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-product',
  standalone: false,

  templateUrl: './product.component.html',
  styleUrl: './product.component.css',
})
export class ProductComponent implements OnInit {
  products: Product[] = [];
  dataLoaded=false
 
  constructor(private productService:ProductService) {}

  ngOnInit(): void {
   this.getProducts()
  }

  getProducts() {
   this.productService.getProducts().subscribe(response=>{
    this.products=response.data;
    this.dataLoaded=true

   })
  }
}
--------------------------------
böylece kodlarımızı refakte etmiş oluyoruz.
--------------------------------
Ama RsponseModel kısmında biraz daha refaktör yapalım. models kısmına sağ tıklıyoruz ve listResponseModel.ts adında bir dosya oluşturuyoruz.
------------------------------
import { ResponseModel } from "./responseModel";

export interface ListResponseModel<T>{
data:T[]
}
------------------------
Biz bunu yapınca artık ProductResponseModel ve ResponseModel'e grek kalmadı bu yüzden onları siliyoruz. Ve bundan sonraki entities lerde bu listResponseModel'i kullanacağız.
tabi bunu yapınca product.component.ts ve product.service.ts dosyaları hata verecek çünkü onlara daha önce kullanılan PrdouctResponseModel ve ResponseModel bulunmakta dolayısıyla onları şu şekilde değiştiriyoruz.
---------------------------
product.service.ts dosyası
----------------------------
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ListResponseModel } from '../models/listResponseModel';
import { Product } from '../models/product';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  apiUrl = 'https://localhost:7206/api/products/getall';


  constructor(private httpClient: HttpClient) { }

   getProducts():Observable<ListResponseModel<Product>> {
     return this.httpClient.get<ListResponseModel<Product>>(this.apiUrl);
       
        }
    }

-------------------------
product.componnt.ts dosyası
--------------------------
import { Component, OnInit } from '@angular/core';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-product',
  standalone: false,

  templateUrl: './product.component.html',
  styleUrl: './product.component.css',
})
export class ProductComponent implements OnInit {
  products: Product[] = [];
  dataLoaded=false
 
  constructor(private productService:ProductService) {}

  ngOnInit(): void {
   this.getProducts()
  }

  getProducts() {
   this.productService.getProducts().subscribe(response=>{
    this.products=response.data;
    this.dataLoaded=true

   })
  }
}
-----------------------
ANGULAR-ROUTİNG
Bu işlem bittikten sonra şimdi rooting olayı üzerinde duralım. program çalıştığında ilk çalışan html sayfası app.component.html sayfasıdır. orada sol tarafta kategoriler üstte navigasyon çubuğu bulunur orta kısım ise sürekli değişecek yerdir. o bölüme       <router-outlet></router-outlet> ifadesini koyuyoruz.
-----------------------
<div class="container">
  <app-navi></app-navi>
  <div class="row">
    <div class="col-md-3">
      <app-category></app-category>
    </div>
    <div class="col-md-9">
      <router-outlet></router-outlet>

    </div>
  </div>
</div>
---------------------------
burada hangi sayfanın gösterileceği app.routing.module.ts dosyasındaki rooting kodları belirleyecek const routes dizisindeki rout ifadeleri routing işlemini oluşturacak.
--------------------------
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProductComponent } from './components/product/product.component';

const routes: Routes = [
  {path:"",pathMatch:"full",component:ProductComponent},
  {path:"products",component:ProductComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
--------------------------------
şimdi sol tarafta seçim yapılan kategoriye göre ürünlerin gösterilmesi işlemini yapalım.
-----------------------------
category.component.ts dosyasında seçilen kategoriyi belirleyen ve onu tutan iki tane fonksiyon oluşturuyoruz.
----------------------------
import { Component, OnInit } from '@angular/core';
import { CategoryService } from '../../services/category.service';
import { Category } from '../../models/category';

@Component({
  selector: 'app-category',
  standalone: false,
  
  templateUrl: './category.component.html',
  styleUrl: './category.component.css'
})
export class CategoryComponent implements OnInit {
  categories:Category[]=[]
  constructor(private categoryService:CategoryService){}
  currentCategory:Category
  // currentCategory:Category şeklinde kullanmak için tesconfig.json dosyasına "strict":"true"'den sonra
  // "strictPropertyInitialization": false, eklemesi yapılır. 

  
  ngOnInit(): void {
this.getCategories()
  }
  getCategories(){
this.categoryService.getCategories().subscribe(response=>{
  this.categories=response.data
})
  }
  setCurrentCategory(category:Category){
   this.currentCategory=category
  }

  getCurrentCategoryClass(category:Category){
   if(category==this.currentCategory){
    return "list-group-item active"
   }
   else{
    return "list-group-item"
   }
  }


}
---------------------------
 currentCategory:Category
  // currentCategory:Category şeklinde kullanmak için tesconfig.json dosyasına "strict":"true"'den sonra
  // "strictPropertyInitialization": false, eklemesi yapılır.  Yada 
    currentCategory:Category={categoryId:0,categoryName:""} şeklinde ilk değeri veriyoruz. ancak bunu yapınca ilk değer boşta olsa verildiğinden dolayı birşey seçili olmasa da seçtiniz yazısı görünür.  category.component.html dosyasını da şu şekilde oluşturuyoruz. bu sebeple  currentCategory:Category şeklinde kullanıyoruz ve  <h5 *ngIf="currentCategory">{{currentCategory.categoryName}} seçtiniz</h5> seçildiği anda bu <h5>'i göster diyoruz.
---------------------------
<ul class="list-group">
    <li (click)="setCurrentCategory(category)" *ngFor="let category of categories
    " [class]="getCurrentCategoryClass(category)">{{category.categoryName}}</li>
   
  </ul>
  <h5 *ngIf="currentCategory">{{currentCategory.categoryName}} seçtiniz</h5>
  --------------------------
  Bu kodlarla birlikte seçildiğinde rengi maviye döner ve aşağıda hangi kategoriyi seçtiğimiz yazılır.
  ----------------------------
  şimdi seçilen kategoriye göre ürünleri getirelim öncelikle routing olarak şu root' u ekliyoruz
  --------------------------
    {path:"products/category/:categoryId",component:ProductComponent}
---------------------------
öncelikle backende seçilen kategoriye göre ürünleri getirmeyi controllerde  eklemek gerekir.
--------------------------
backendde şu eklemeyi yapıyoruz.
---------------------------
  [HttpGet("getallBycategoryid")]
  public IActionResult GetAllByCategoryId(int id)
  {
      var result = _productService.GetAllByCategoryId(id);
      if (result.IsSuccess)
      {
          return Ok(result);
      }
      return BadRequest(result);
  }
  -------------------------
backend de bunu yapmışken şunuda ekleyelim.
----------------------------
 [HttpGet("getproductdetails")]
 public IActionResult GetProductDetails()
 {
     var result = _productService.GetProductDetails();
     if (result.IsSuccess)
     {
         return Ok(result);
     }
     return BadRequest(result);
 }
 ------------------------
Backendde bu değişikliği yaptıktan sonra angulara geçiyoruz ve product.service.ts dosyasında 
öncelikle api adresinde bir değişiklik yapıyoruz. ve her türlü durumda kullanılan api adresine çeviriyoruz.
-----------------------
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ListResponseModel } from '../models/listResponseModel';
import { Product } from '../models/product';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  apiUrl = 'https://localhost:7206/api/';


  constructor(private httpClient: HttpClient) { }

   getProducts():Observable<ListResponseModel<Product>> {
    let newPath=this.apiUrl+"products/getall"
     return this.httpClient.get<ListResponseModel<Product>>(newPath);       
        }
        getProductsByCategory(categoryId:number):Observable<ListResponseModel<Product>>{
          let newPath=this.apiUrl+"products/getallbycategoryid?categoryId="+categoryId
          return this.httpClient.get<ListResponseModel<Product>>(newPath)
        }
    }

---------------------------
product.component.ts dosyası şu şekilde düzenleniyor.
----------------------------
import { Component, OnInit } from '@angular/core';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-product',
  standalone: false,

  templateUrl: './product.component.html',
  styleUrl: './product.component.css',
})
export class ProductComponent implements OnInit {
  products: Product[] = [];
  dataLoaded=false
 
  constructor(private productService:ProductService,private activatedRoute:ActivatedRoute) {}

  ngOnInit(): void {
  this.activatedRoute.params.subscribe(params=>{
    if(params["categoryId"]){
      this.getProductsByCategory(params["categoryId"])
    }else{
      this.getProducts()
    }
  })
  }

  getProducts() {
   this.productService.getProducts().subscribe(response=>{
    this.products=response.data;
    this.dataLoaded=true

   })
  }
  getProductsByCategory(categoryId:number){
    this.productService.getProductsByCategory(categoryId).subscribe(response=>{
      this.products=response.data
      this.dataLoaded=true
    })
  }
}
--------------------------
app.routing.module.ts dosyası da şu şekilde oluyor.
--------------------------
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProductComponent } from './components/product/product.component';

const routes: Routes = [
  {path:"",pathMatch:"full",component:ProductComponent},
  {path:"products",component:ProductComponent},
  {path:"products/category/:categoryId",component:ProductComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
-------------------------
Böylece solda seçilen kategori ne ise o kategoriden olan ürünler ekrana gelir.
Şimdi çalışmayan Dropdon menüsünü çalıştıralım. Öncelikle npm install jquery diyerek jquery paketini yüklüyoruz. Daha sonra aktif hale getirmek için angular.json dosyasında ilk scripts[] alanına
-------------------------
 "scripts": [
               "./node_modules/bootstrap/dist/js/bootstrap.min.js",
               "./node_modules/jquery/dist/jquery.min.js"
            ]
            -----------------------
eklemesi yapıyoruz. Ama angular 18 de çalışmadı. Yeni versiyon angularda çalışması için 
---------------------------
 "styles": [
              "./node_modules/bootstrap/dist/css/bootstrap.min.css",             
              "src/styles.css"
            ],
            "scripts": [
              "./node_modules/@popperjs/core/dist/umd/popper.min.js",
               "./node_modules/bootstrap/dist/js/bootstrap.bundle.min.js"
---------------------------
eklenmesi gerekiyor. Şimdi elimizdeki datayı farklı bir şekilde göstermek için PIPE konusuna değineceğiz

PIPE
------------------
app klasörü içine pipes adında yeni bir klasör oluşturuyoruz.pipe klasörü üzerindeyken open in integrated terminal diyerek pipe klasörü içine giriyoruz. ng g pipe vatAdded adında yeni bir pipe oluşturuyoruz. Bu yazacağımız custom bir pipe olacak bunun yanında angular içinde hazır olarak kullanılan built-in pipeler de mevcuttur. Pipeler datayı farklı göstermek istediğimiz html dosyalarında kullanılır. component klasöründ product.component.html dosyasında şu şekilde kullanıyoruz. mesela product isimlerini büyük harfle göstermek istersek
-----------------------
<td>{{ product.productName | uppercase }}</td>
-----------------------
şeklinde kullanılır. Ama biz kendimiz bir tane pipe oluşturalım mesela biza api'den KDV'li fiyat gelmiyor biz bir pipe vasıtası ile ürünün KDV'li fiyatını hesaplayıp ekranda gösterelim.daha önce oluşturduğumuz vatAdded pipe'inde bu hesaplamayı yapacak kodu yazacağız.
---------------------------
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'vatAdded',
  standalone: false
})
export class VatAddedPipe implements PipeTransform {

  transform(value: number, rate: number): number {
    return value + (value*rate/100);
  }

}
-------------------------
kullanımı da şu şekilde olacak
----------------------------
<div *ngIf="dataLoaded==false" class="spinner-border" role="status">
    <span class="visually-hidden">Loading...</span>
  </div>
<table *ngIf="dataLoaded==true" class="table">
  <thead>
    <tr>
      <th>Ürün Id</th>
      <th>Kategori Id</th>
      <th>Ürün Adı</th>
      <th>Fiyatı</th>
      <th>Kdv'li Fiyat</th>
      <th>Stok Sayısı</th>
    </tr>
  </thead>

  <tr *ngFor="let product of products">
    <td>{{ product.productId }}</td>
    <td>{{ product.categoryId }}</td>
    <td>{{ product.productName | titlecase }}</td>
    <td>{{ product.unitPrice | currency:"TRY":"TL ":""}}</td>
    <td>{{ product.unitPrice | vatAdded:18 | currency}}</td>
    <td>{{ product.unitsInStock }}</td>
  </tr>
</table>
----------------------------

Şimdi bir pipe daha yazacağız ve bu pipe'in görevi ürün araması yapmak olacak Bootstrap sayfasından Forms menüsünden Form-Control kısmından
--------------------------
<div class="mb-3">
  <label for="exampleFormControlInput1" class="form-label">Email address</label>
  <input type="email" class="form-control" id="exampleFormControlInput1" placeholder="name@example.com">
</div>
--------------------------
kısmı kopyalıyoruz. product.component.html dosyasında table'in üstüne yapıştırıyoruz. ama önce şu şekilde değişiklikler yapıyoruz type'ini emailden Text'e çeviriyoruz ve id'sini filterText olarak değiştiriyoruz. ve Label kısmını da Ürün ara Place holder kısmını ürün adı yaz şeklinde değişitiriyoruz.

-------------------------
 <div class="mb-3">
    <label for="filterText" class="form-label">Ürün Ara</label>
    <input type="text" class="form-control" id="filterText" placeholder="Ürün adı yaz">
  </div>
  <div>
    Burada ise şu ürünü aradınız şeklinde bir uyarı göstereceğiz.
  </div>
------------------------
Bunu yapabilmek için daha doğrusu angular da bir data kullanacaksak bunu önce component.ts dosyasında tanımlamamız gerekir.
------------------------
import { Component, OnInit } from '@angular/core';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-product',
  standalone: false,

  templateUrl: './product.component.html',
  styleUrl: './product.component.css',
})
export class ProductComponent implements OnInit {
  products: Product[] = [];
  dataLoaded=false
  filterText=""
 
  constructor(private productService:ProductService,private activatedRoute:ActivatedRoute) {}

  ngOnInit(): void {
  this.activatedRoute.params.subscribe(params=>{
    if(params["categoryId"]){
      this.getProductsByCategory(params["categoryId"])
    }else{
      this.getProducts()
    }
  })
  }

  getProducts() {
   this.productService.getProducts().subscribe(response=>{
    this.products=response.data;
    this.dataLoaded=true

   })
  }
  getProductsByCategory(categoryId:number){
    this.productService.getProductsByCategory(categoryId).subscribe(response=>{
      this.products=response.data
      this.dataLoaded=true
    })
  }
}
--------------------------
ama buradaki filterText ile html dosyasındaki filterText'i birbiriyle ilişkilendirmek gerekir bunu yapmak için html dosyasında [(ngModel)] notasyonu kullanılır
-------------------------
 <div class="mb-3">
    <label for="filterText" class="form-label">Ürün Ara</label>
    <input  type="text"  [(ngModel)]="filterText" class="form-control" id="filterText" placeholder="Ürün adı yaz">
  </div>
  ---------------------------
  ama anguların yeni versiyonlarında bunun kullanılabilmesi için app.module.ts dosyasına FormsModule modulünün eklenmesi gerekir.
  ---------------------------
  import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ProductComponent } from './components/product/product.component';
import { CategoryComponent } from './components/category/category.component';
import { NaviComponent } from './components/navi/navi.component';
import { provideHttpClient } from '@angular/common/http';
import { VatAddedPipe } from './pipes/vat-added.pipe';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    AppComponent,
    ProductComponent,
    CategoryComponent,
    NaviComponent,
    VatAddedPipe
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule
  ],
  providers: [
    provideHttpClient()
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
------------------------


Bu işlem sadece basitçe arama kutusuna yazılan yazının alttaki form kutusunda gösterilmesini sağlar oysa bizim yapmak istediğimiz üstteki arama kutusuna ürün ismi yazdığımızda o isimdeki ürün yada ürünlerin getirilmesidir. Bun yapmak için bir pipe daha yazacağız bu pipe'in ismini filterPipe olarak koyuyoruz. burada value olarak yani değiştirilemsi istenen değer olarak Product listesidir. değiştirilen değer ise yazılan string olduğundan string dir. Bu tür arama işlemlerinde öncelikle aranacak veriyi ya büyük harfe yada küçük harfe çevirmek olmalıdır bunu yapmazsak yazılan büyük yada küçük harfe göre aramanın etkilenmesidir çünkü javascript büyük küçük harf duyarlıdır. bu pipe üzerinede java script kodu yazacağız javascriptin filter özelliğinden yararlanacağız. Kodumuz şu şekilde olacak.
--------------------------
import { Pipe, PipeTransform } from '@angular/core';
import { Product } from '../models/product';

@Pipe({
  name: 'filterPipe',
  standalone: false
})
export class FilterPipePipe implements PipeTransform {

  transform(value: Product[], filterText: string): Product[] {
    filterText=filterText?filterText.toLocaleLowerCase():""
    return filterText?value.filter((p:Product)=>p.productName.toLocaleLowerCase().indexOf(filterText)!==-1) :value
  }

}
-------------------------
Bunu peki nerede kullanacağız burada değiştirilmek istenen product listesi olduğundan arama yapılacak satıra değil ürünü etkileyn tüm tabloya(satıra) uygulanmalıdır.
-------------------------
<div *ngIf="dataLoaded==false" class="spinner-border" role="status">
    <span class="visually-hidden">Loading...</span>
  </div>

  <div class="mb-3">
    <label for="filterText" class="form-label">Ürün Ara</label>
    <input  type="text"  [(ngModel)]="filterText" class="form-control" id="filterText" placeholder="Ürün adı yaz">
  </div>

  <div *ngIf="filterText" class="alert alert-success">
{{filterText}} aradınız.
  </div>
<table *ngIf="dataLoaded==true" class="table">
  <thead>
    <tr>
      <th>Ürün Id</th>
      <th>Kategori Id</th>
      <th>Ürün Adı</th>
      <th>Fiyatı</th>
      <th>Kdv'li Fiyat</th>
      <th>Stok Sayısı</th>
    </tr>
  </thead>

  <tr *ngFor="let product of products | filterPipe:filterText">
    <td>{{ product.productId }}</td>
    <td>{{ product.categoryId }}</td>
    <td>{{ product.productName | titlecase }}</td>
    <td>{{ product.unitPrice | currency:"TRY":"TL ":""}}</td>
    <td>{{ product.unitPrice | vatAdded:18 | currency:"TRY":"TL ":""}}</td>
    <td>{{ product.unitsInStock }}</td>
  </tr>
</table>
------------------------

Şimdi sepet işlemleriyle ilgili çalışmalar yapacağız.product.component.html dosyasında ensona bir buton ekleyeceğiz bunu yapmak için öncelikle başlık bölümünde boş bir <th></th> ekliyoruz. bunu eklemeden de alt kısımdaki satırın sonuna bir buton eklenir ancak görüntü bozulabilir diye bu <th></th>yi eklemek daha iyi sonuç verir. Bundan sonra listemizin sonuna buton eklemek için yeni bir <td></td> ekliyoruz ve içine <button></button> ekliyoruz. 
    <td><button type="button" class="btn btn-success">Sepete ekle</button></td>
ancak bu buton düzgün görünmüyor bu yüzden <tbody></tbody> içine satırı alarak sorunu çözüyoruz.
-------------------------------
<tbody>
  <tr *ngFor="let product of products | filterPipe:filterText">
    <td>{{ product.productId }}</td>
    <td>{{ product.categoryId }}</td>
    <td>{{ product.productName | titlecase }}</td>
    <td>{{ product.unitPrice | currency:"TRY":"TL ":""}}</td>
    <td>{{ product.unitPrice | vatAdded:18 | currency:"TRY":"TL ":""}}</td>
    <td>{{ product.unitsInStock }}</td>
    <td><button type="button" class="btn btn-success">Sepete ekle</button></td>
  </tr>
</tbody>
----------------------
böylece sorun çözülüyor. Şimdi bu butona bir event binding yapıyoruz. click özelliğini kullanıyoruz. (click)="addToCart(product)" bu şu demek bu butona tıklandığında component.ts dosyasındaki addToCart(product) fonksiyonunu çalıştır. Sepete ekle ye bazınca bize uyarı verecek bir notificasyon özelliği ekleyelim. bunun için cmd satırına npm install ngx-toastr yazarak toastr notification özelliğini ekliyoruz. Bu toastr bir animasyon paketini kullanıyor bu sebeple onuda ekliyoruz. npm install @angular/animations@19.0.5 yazarak bu paketi de ekliyoruz. Tabi toastr kullanabilmek için angular.json dosyasının still bölümüne bunun ifadsinin eklenmesi gerekiyor.
              "./node_modules/ngx-toastr/toastr.css",

daha sonraki adım app.module.ts dosyasına da import edilmesi gerekiyor.
import { ToastrModule } from 'ngx-toastr';
 ve imports kısmına da eklenmesi gerekiyor. ancak eklerken şu şekilde ekliyoruz     ToastrModule.forRoot({
      positionClass:"toast-bottom-right",
      
    })
bu bize roottan itibaren kullanılabilir hale getir demek ayrıca sağ altta göster demek
-------------------------
 imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    ToastrModule.forRoot({
      positionClass:"toast-bottom-right",
      
    })
  ],
  -------------------------
ngx-toastr sitesine giderek başka hangi özellikleri uygulanabilir oradan bakılabilir. Şimdi artık toastr kullanabiliriz. sepete ekleme esnasında kullanmak istiyoruz.product.component.ts dosyasında constructor bölümünde önce servisini ekliyoruz.
-------------------------
import { Component, OnInit } from '@angular/core';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-product',
  standalone: false,

  templateUrl: './product.component.html',
  styleUrl: './product.component.css',
})
export class ProductComponent implements OnInit {
  products: Product[] = [];
  dataLoaded=false
  filterText=""
 
  constructor(private productService:ProductService,private activatedRoute:ActivatedRoute,private toastrService:ToastrService) {}

  ngOnInit(): void {
  this.activatedRoute.params.subscribe(params=>{
    if(params["categoryId"]){
      this.getProductsByCategory(params["categoryId"])
    }else{
      this.getProducts()
    }
  })
  }

  getProducts() {
   this.productService.getProducts().subscribe(response=>{
    this.products=response.data;
    this.dataLoaded=true

   })
  }
  getProductsByCategory(categoryId:number){
    this.productService.getProductsByCategory(categoryId).subscribe(response=>{
      this.products=response.data
      this.dataLoaded=true
    })
  }

  addToCart(product:Product){
    this.toastrService.success("Sepete eklendi.",product.productName)
  }
}
---------------------------
Toastr ın çalışması için BrowserAnimationModule'ünde app.module.ts dosyasına import olarak eklenmesi gerekiyor.
----------------------------
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ProductComponent } from './components/product/product.component';
import { CategoryComponent } from './components/category/category.component';
import { NaviComponent } from './components/navi/navi.component';
import { provideHttpClient } from '@angular/common/http';
import { VatAddedPipe } from './pipes/vat-added.pipe';
import { FormsModule } from '@angular/forms';
import { FilterPipePipe } from './pipes/filter-pipe.pipe';
import { ToastrModule } from 'ngx-toastr';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';


@NgModule({
  declarations: [
    AppComponent,
    ProductComponent,
    CategoryComponent,
    NaviComponent,
    VatAddedPipe,
    FilterPipePipe
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    ToastrModule.forRoot({
      positionClass:"toast-bottom-right",
    }),
    BrowserAnimationsModule
    
  ],
  providers: [
    provideHttpClient()
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
----------------------------

Şimdi bizim burada yaptığımız sepete ekle butonuna basınca ürün sepete eklendi uyarısı geliyor ancak bizim yapmak istediğimiz sepete ürün ekleye basınca uyarının yanında üründe gerçekten sepete eklensin ama bunun için farklı yöntemler mevcut bunu veritabanı tablosu olarak oluşturabildiğimiz gibi bunu sadece frontendde tutabiliriz. amacımıza göre bu yöntem değişebilir biz bunu sadece angular üzerinde yapacağız bunun için öncelikle bir modelini oluşturmamız gerekiyor. model klasörüne geliyoruz. cartItem adında bir interface oluşturuyoruz.
----------------------------
import { Product } from "./product";

export interface CartItem{
    product:Product
    quantity:number
}
------------------------------
ve bir cartItems adında bir sabit oluşturuyoruz.
------------------------------
import { CartItem } from "./cartitem";

export const CartItems:CartItem[]=[]
-------------------------------
şimdi components klasörüne open in integrated ile giriyoruz ve ng g component cart-summary adında bir component oluşturuyoruz. navi.component.html dosyasında bulunan dropdown'u oradan kesiyoruz ve cart-summary.component.html sayfasına yapıştırıyoruz. kestiğimiz yere ise buranın selectorunu yani <app-cart-summary> <app-cart-summary> yazıyoruz. ve cart-summary.component.html sayfasına yapıştırdığımız bu kodu sepet sayfası olarak düzenliyoruz.
----------------------------
<li *ngIf="cartItems" class="nav-item dropdown">
  <a
    class="nav-link dropdown-toggle"
    href="#"
    role="button"
    data-bs-toggle="dropdown"
    aria-expanded="false"
  >
    Sepetim
  </a>
  <ul class="dropdown-menu">
    <li  *ngFor="let cartItem of cartItems" >
      <a class="dropdown-item">{{
        cartItem.product.productName
      }}</a>
    </li>
    <li><hr class="dropdown-divider" /></li>
    <li><a class="dropdown-item">Sepete git</a></li>
  </ul>
</li>
-----------------------------
şimdi ise yapacağımız işlem sepete ekle butonuna basınca burada eklenen ürün isimlerini görmek olacak bunu tek bir sayfada değil hemen hemen ürünle ilgili her sayfada kullanacağımız için sepet işlemlerini halledecek bir servis yazıyoruz. services klasörüne open in integrated ile giriyoruz ve ng g service cart ile cart adında bir servis oluşturuyoruz.ve içinde addToCart(product:Product) adında bir fonksiyon yazıyoruz yanlız burada şunu yapacağız. diyelimki aynı ürünü bir kez daha eklemek istediğimiz de quantity sini bir arttırsın dolayısıyla öncelikle eklemek istediğimiz ürün sepette varmı yokmu kontrol etmemiz gerekiyor.
----------------------------
import { Injectable } from '@angular/core';
import { Product } from '../models/product';
import { CartItems } from '../models/cartItems';
import { CartItem } from '../models/cartItem';

@Injectable({
  providedIn: 'root'
})
export class CartService {

  constructor() { }

  addToCart(product:Product){
    let item=CartItems.find(c=>c.product.productId===product.productId)
    if(item){
      item.quantity+=1
    }else{
      let cartItem=new CartItem()
      cartItem.product=product
      cartItem.quantity=1
      CartItems.push(cartItem)
    }
  }

  list(){
    return CartItems
  }
}
------------------------
şimdi bu servisi kullanalım bunu product.component.ts dosyasında kullanıyoruz.
-----------------------
import { Component, OnInit } from '@angular/core';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-product',
  standalone: false,

  templateUrl: './product.component.html',
  styleUrl: './product.component.css',
})
export class ProductComponent implements OnInit {
  products: Product[] = [];
  dataLoaded = false;
  filterText = '';

  constructor(
    private productService: ProductService,
    private activatedRoute: ActivatedRoute,
    private toastrService: ToastrService,
    private cartService:CartService
  ) {}

  ngOnInit(): void {
    this.activatedRoute.params.subscribe((params) => {
      if (params['categoryId']) {
        this.getProductsByCategory(params['categoryId']);
      } else {
        this.getProducts();
      }
    });
  }

  getProducts() {
    this.productService.getProducts().subscribe((response) => {
      this.products = response.data;
      this.dataLoaded = true;
    });
  }
  getProductsByCategory(categoryId: number) {
    this.productService
      .getProductsByCategory(categoryId)
      .subscribe((response) => {
        this.products = response.data;
        this.dataLoaded = true;
      });
  }

  addToCart(product: Product) {
    this.cartService.addToCart(product)
    this.toastrService.success('Sepete eklendi.', product.productName);
  }
}
------------------------
cart-summary.component.ts dosyası şu şekilde olacak
-------------------------
import { Component, OnInit } from '@angular/core';
import { CartItem } from '../../models/cartItem';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-cart-summary',
  standalone: false,
  
  templateUrl: './cart-summary.component.html',
  styleUrl: './cart-summary.component.css'
})
export class CartSummaryComponent implements OnInit {

  cartItems:CartItem[]

  constructor(private cartService:CartService){}
  ngOnInit(): void {
   this.getCart()
  }

  getCart(){
    this.cartItems=this.cartService.list()
  }

}
------------------------
cart-summary.componnt.html şu şekilde olacak
--------------------------
<li *ngIf="cartItems.length>0" class="nav-item dropdown">
  <a
    class="nav-link dropdown-toggle"
    href="#"
    role="button"
    data-bs-toggle="dropdown"
    aria-expanded="false"
  >
    Sepetim
  </a>
  <ul class="dropdown-menu">
    <li  *ngFor="let cartItem of cartItems"  >
      <a class="dropdown-item">{{
        cartItem.product.productName
      }} <span class="badge text-bg-secondary">{{cartItem.quantity}}</span></a>
    </li>
    <li><hr class="dropdown-divider" /></li>
    <li><a class="dropdown-item">Sepete git</a></li>
  </ul>
</li>
-----------------------
şimdi sepetten ürün silme işlemi yapacağız öncelikle sepete eklenen ürünün hemen yanına sil ibaresi koyuyoruz.
-------------------------
<li *ngIf="cartItems.length>0" class="nav-item dropdown">
  <a
    class="nav-link dropdown-toggle"
    href="#"
    role="button"
    data-bs-toggle="dropdown"
    aria-expanded="false"
  >
    Sepetim
  </a>
  <ul class="dropdown-menu">
    <li  *ngFor="let cartItem of cartItems"  >
      <a class="dropdown-item">{{
        cartItem.product.productName
      }} <span class="badge text-bg-danger">{{cartItem.quantity}}</span> <span  class="badge text-bg-secondary"> Sil</span></a>
    </li>
    <li><hr class="dropdown-divider" /></li>
    <li><a class="dropdown-item">Sepete git</a></li>
  </ul>
</li>
--------------------------
şimdi de cart servisinde silme fonksiyonunu yazıyoruz
--------------------------
removeFromCart(product:Product){
    let item=CartItems.find(p=>p.product.productId===product.productId)
   CartItems.splice(CartItems.indexOf(item),1)
  }
  -------------------------
  yalnız bunu yapınca type scriptin tip güvenliğinden dolayı hata veriyor. bu sebeple tsconfig.json dosyasında "strict" 'in altına  "strictNullChecks": false, eklemesini yapıyoruz. bunu yapınca hata ortadan kalkıyor. Bu servisi cart-summary.component.ts dosyasında kullanıyoruz.
  -----------------------
  import { Component, OnInit } from '@angular/core';
import { CartItem } from '../../models/cartItem';
import { CartService } from '../../services/cart.service';
import { ToastrService } from 'ngx-toastr';
import { Product } from '../../models/product';

@Component({
  selector: 'app-cart-summary',
  standalone: false,
  
  templateUrl: './cart-summary.component.html',
  styleUrl: './cart-summary.component.css'
})
export class CartSummaryComponent implements OnInit {

  cartItems:CartItem[]

  constructor(private cartService:CartService,private toastrService:ToastrService){}
  ngOnInit(): void {
   this.getCart()
  }

  getCart(){
    this.cartItems=this.cartService.list()
  }
  removeFromCart(product:Product){
    this.cartService.removeFromCart(product)
    this.toastrService.error("Sepetten bir ürün silindi.",product.productName)
  }


}
---------------------------
Bunu cart-summary.component.html dosyasında click olarak bu fonksiyona çağrı yapılacak
--------------------------
<li *ngIf="cartItems.length > 0" class="nav-item dropdown">
  <a
    class="nav-link dropdown-toggle"
    href="#"
    role="button"
    data-bs-toggle="dropdown"
    aria-expanded="false"
  >
    Sepetim
  </a>
  <ul class="dropdown-menu">
    <li *ngFor="let cartItem of cartItems">
      <a class="dropdown-item"
        >{{ cartItem.product.productName }}
        <span class="badge text-bg-danger">{{ cartItem.quantity }}</span>
        <span
          (click)="removeFromCart(cartItem.product)"
          class="badge text-bg-secondary"
        >
          Sil</span
        ></a
      >
    </li>
    <li><hr class="dropdown-divider" /></li>
    <li><a class="dropdown-item">Sepete git</a></li>
  </ul>
</li>
-------------------------

Şimdi reaktif formlar konusuna geçiyoruz. yani veri girme işlemlerini yapacağız.Şimdi ürün ekleyecek bir ortam oluşturalım. Reaktif formların kullanılabilmesi için öncelikle bizim daha önce eklediğimiz FormsModule'nin eklenmesi gerekir. Yine bunun yanında reaktif formların kullanılabilmesi için reaktiveFormsModule'ünde app.module.ts de eklenmesi gerekir.Şimdi components klasörünün içine girerek product-add adında component oluştur.
--------------------------
import { FormsModule,ReactiveFormsModule } from '@angular/forms';


 imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    ToastrModule.forRoot({
      positionClass:"toast-bottom-right",
    }),
    BrowserAnimationsModule
    
  ],

  -------------------------
  Bunun dışında reactive formlarla çalışabilmek için product-add.component.ts dosyasında bazı importları girmemiz gerekiyor.
  -----------------------
  import { FormGroup,FormBuilder,FormControl,Validators } from '@angular/forms';
--------------------------
FormBuilder bir servis olduğundan constructor() kısmına eklenmesi gerekiyor.
-----------------------
 constructor(private formBuilder:FormBuilder){}
----------------------------
product-add.component.ts dosyasında createProductAddForm(){} fonksiyonu ile html dosyasındaki formda hangi verilerin map edileceğini burada oluşturuyoruz.
-------------------------
 createProductAddForm(){
    this.productAddForm=this.formBuilder.group({
      productName:["",Validators.required],
      unitPrice:["",Validators.required],
      unitsInStock:["",Validators.required],
      categoryId:["",Validators.required]
    })
  }
  ------------------------
  şimdi bunun html kısmını oluşturuyoruz.
  -------------------------
<div class="content">
  <div class="col-md-12">
    <div class="card">
      <div class="card-header"><h5>Ürün Ekleme</h5></div>
      <div class="card-body">
        <form [formGroup]="productAddForm" ngForm="add()">
          <div class="mb-3">
            <label for="productName"><h6>Ürün Adı</h6></label>
            <input
              class="form-control"
              type="text"
              id="productName"
              formControlName="productName"
              placeholder="ürün adı giriniz"
            />
          </div>
          <div class="mb-3">
            <label for="unitPrice"><h6>Fiyatı</h6></label>
            <input
              class="form-control"
              type="number"
              id="unitPrice"
              formControlName="unitPrice"
              placeholder="fiyatı giriniz"
            />
          </div>
          <div class="mb-3">
            <label for="unitsInStock"><h6>Stok Adedi</h6></label>
            <input
              class="form-control"
              type="number"
              id="unitsInStock"
              formControlName="unitsInStock"
              placeholder="stok giriniz"
            />
          </div>
          <div class="mb-3">
            <label for="categoryId"><h6>Kategori</h6></label>
            <select
              class="form-select"
              aria-label="Default select example"
              id="categoryId"
              formControlName="categoryId"             
            >
            <option selected>
             Kategori seçiniz
            </option>
              <option *ngFor="let category of categories" [ngValue]="category.categoryId">
                {{ category.categoryName }}
              </option>
            </select>
          </div>         
        </form>
      </div>
      <div class="card-footer">
        <button type="submit" class="btn btn-fill btn-primary" (click)="add()">Kaydet</button>
      </div>
    </div>
  </div>
</div>

-------------------------
Buradaki formControlName product-add.component.ts ile maplemeyi sağlar. Buttona basılınca product-add.component.ts dosyasındaki (click)="add()" fonksiyonu çalışır.
veritabanına girdiğimiz ürün bilgilerini gireceğiz. biz bu işlemler için servis kullanıyoruz.
-------------------------
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ListResponseModel } from '../models/listResponseModel';
import { Product } from '../models/product';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  apiUrl = 'https://localhost:7206/api/';


  constructor(private httpClient: HttpClient) { }

   getProducts():Observable<ListResponseModel<Product>> {
    let newPath=this.apiUrl+"products/getall"
     return this.httpClient.get<ListResponseModel<Product>>(newPath);       
        }
        getProductsByCategory(categoryId:number):Observable<ListResponseModel<Product>>{
          let newPath=this.apiUrl+"products/getallbycategoryid?categoryId="+categoryId
          return this.httpClient.get<ListResponseModel<Product>>(newPath)
        }
        add(product:Product){
         return this.httpClient.post(this.apiUrl+"products/add",product)
        }
    }

--------------------------
son olarak product-add.component.ts dosyasında add() fonksiyonunu yazıyoruz.
---------------------------
import { Component, OnInit } from '@angular/core';
import {
  FormGroup,
  FormBuilder,
  FormControl,
  Validators,
} from '@angular/forms';
import { Category } from '../../models/category';
import { CategoryService } from '../../services/category.service';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-product-add',
  standalone: false,

  templateUrl: './product-add.component.html',
  styleUrl: './product-add.component.css',
})
export class ProductAddComponent implements OnInit {
  productAddForm: FormGroup;
  categories: Category[];

  constructor(
    private formBuilder: FormBuilder,
    private categoryService: CategoryService,
    private productService: ProductService,
    private toastrService:ToastrService
  ) {}

  ngOnInit(): void {
    this.getCategories();
    this.createProductAddForm();
  }

  createProductAddForm() {
    this.productAddForm = this.formBuilder.group({
      productName: ['', Validators.required],
      unitPrice: ['', Validators.required],
      unitsInStock: ['', Validators.required],
      categoryId: ['', Validators.required],
    });
  }
  getCategories() {
    this.categoryService.getCategories().subscribe((response) => {
      this.categories = response.data;
    });
  }
  add() {
    if(this.productAddForm.valid){
      let productModel = Object.assign({},this.productAddForm.value)
      this.productService.add(productModel).subscribe(response=>{
        productModel=response
      },responseError=>{
        this.toastrService.error("Bu ürün eklenemez",responseError.error)
      }) 
          this.toastrService.success("Bir ürün eklendi",productModel.productName)
    }else{
      this.toastrService.error("Ürün ekleme sırasında hata oluştu")
    }
  
  }
}
------------------------
Burada herhangi bir hata oluştuğunda hatayı yakaladık ancak backend'de genel bir hata yakalama(Try-catch) yapacağız.Bu sebeple şimdi öncelikle backend'de bu işlemi yapalım.
core katmanındaki extensions klasörüne exceptionmiddleware adında bir class oluşturuyoruz.
-------------------------
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extensions
{
    public class ExceptionMiddleware
    {
        private RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception e)
            {
                await HandleExceptionAsync(httpContext, e);
            }
        }
        private Task HandleExceptionAsync(HttpContext httpContext, Exception e)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            string message = "Internal Server Error";
            if (e.GetType() == typeof(ValidationException))
            {
                message = e.Message;
            }
            return httpContext.Response.WriteAsync(new ErrorDetails
            {
                StatusCode = httpContext.Response.StatusCode,
                Message = message
            }.ToString());
        }
    }
}
-------------------------
Ayrıca extensions klasörüne ErrorDetails classı oluşturuyoruz.
-------------------------
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extensions
{
    public class ErrorDetails
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
---------------------------
Burada JsonConvert'in çalışması için NewtonSoft paketinin kurulması gerekiyor.
---------------------------
Birde extensions klasörüne ExceptionMiddlewareExtensions adında bir class oluşturulur.
---------------------------using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
--------------------------
Bundan sonra webAPI katmanındaki program.cs dosaysına
---------------------------
app.ConfigureCustomExceptionMiddleware();
---------------------------
eklenmesi gerekiyor.
------------------------------
Şimdi exceptionmiddleware dosyasını biraz refaktör edelim.
----------------------------
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extensions
{
    public class ExceptionMiddleware
    {
        private RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception e)
            {
                await HandleExceptionAsync(httpContext, e);
            }
        }
        private Task HandleExceptionAsync(HttpContext httpContext, Exception e)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            string message = "Internal Server Error";
            IEnumerable<ValidationFailure> errors;
            if (e.GetType() == typeof(ValidationException))
            {
                message = e.Message;
                errors=((ValidationException)e).Errors;
                httpContext.Response.StatusCode = 400;
                return httpContext.Response.WriteAsync(new ValidationErrorDetails
                {
                    Message = message,
                    StatusCode = 400,
                    ValidationErrors = errors
                }.ToString());
            }
            return httpContext.Response.WriteAsync(new ErrorDetails
            {
                StatusCode = httpContext.Response.StatusCode,
                Message = message
            }.ToString());
        }
    }
}
---------------------------
tabi bunu yapınca ErrorDetails classına da bir property eklememiz gerekiyor. Ayrıca sistem hatasıyla validasyon hatasını biribirinden ayırmak için yani sistem hatasıysa Errors'u frontende göndermemek için ValidationErrorDetails classını ekliyoruz.
----------------------------
using FluentValidation.Results;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extensions
{
    public class ErrorDetails
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    public class ValidationErrorDetails : ErrorDetails()
    {
        public IEnumerable<ValidationFailure> ValidationErrors { get; set; }
    }
}

-----------------------------
Frontend tarafında visualcode da product-add.component.ts dosyasını şu şekilde düzenliyoruz.
---------------------------
import { Component, OnInit } from '@angular/core';
import {
  FormGroup,
  FormBuilder,
  FormControl,
  Validators,
} from '@angular/forms';
import { Category } from '../../models/category';
import { CategoryService } from '../../services/category.service';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-product-add',
  standalone: false,

  templateUrl: './product-add.component.html',
  styleUrl: './product-add.component.css',
})
export class ProductAddComponent implements OnInit {
  productAddForm: FormGroup;
  categories: Category[];

  constructor(
    private formBuilder: FormBuilder,
    private categoryService: CategoryService,
    private productService: ProductService,
    private toastrService: ToastrService
  ) {}

  ngOnInit(): void {
    this.getCategories();
    this.createProductAddForm();
  }

  createProductAddForm() {
    this.productAddForm = this.formBuilder.group({
      productName: ['', Validators.required],
      unitPrice: ['', Validators.required],
      unitsInStock: ['', Validators.required],
      categoryId: ['', Validators.required],
    });
  }
  getCategories() {
    this.categoryService.getCategories().subscribe((response) => {
      this.categories = response.data;
    });
  }
  add() {
    if (this.productAddForm.valid) {
      let productModel = Object.assign({}, this.productAddForm.value);
      this.productService.add(productModel).subscribe(
        (response) => {
          productModel = response;
          this.toastrService.success(
            'Bir ürün eklendi',
            productModel.productName
          );
        },
        (responseError) => {
          if (responseError.error.ValidationErrors.length > 0) {
            for (
              let i = 0;
              i < responseError.error.ValidationErrors.length;
              i++
            ) {
              this.toastrService.error(
                responseError.error.ValidationErrors[i].ErrorMessage,
               "Doğrulama Hatası"
              );
            }
          }
        }
      );
    } else {
      this.toastrService.error('Ürün ekleme sırasında hata oluştu');
    }
  }
}
-------------------------
Böylece artık Toaster da hata mesajları görülmeye başlanır.


KİŞİ ID'SİNE GÖRE PROFİL FOTOĞRAFI EKLEME(hANE YÖNETİMİ PROESİNDE)
--------------------------------------------------------------------
Şimdi senaryo şu şekilde kuruldu yeni bir kişi kaydedilirken fotoğrafsız olarak kaydediliyor ama başka bir component yoluyla istenen herhangi bir kişiye(ID'ye göre) profil fotoğrafı ekleniyor.

Öncelikle familyPerson(API'ye göre) adında bir model oluşturuluyor.
--------------------------
export interface FamilyPerson{
    id:number
    fullName:string
    email:string
    password:string
    profilePicture:string
    roleId:number 
    
}
-----------------------------
Daha sonra ekrana tüm familyPerson listesini getiren ve onun altında da yeni bir kişi klenmesini sağlayan familyPerson.componenti oluşturuluyor.
----------------------------
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FamilyPersonService } from '../../services/family-person.service';
import { FamilyPerson } from '../../models/familyPerson';
import { Role } from '../../models/role';
import { RoleService } from '../../services/role.service';

@Component({
  selector: 'app-family-person',
  templateUrl: './family-person.component.html',
  styleUrls: ['./family-person.component.css'],
  standalone: false,
})
export class FamilyPersonComponent implements OnInit {
  familyPersons:FamilyPerson[]
  roles:Role[]

  // 1) Kişi ekleme formu
  personForm!: FormGroup;

  // 2) Fotoğraf yükleme için input
  selectedFile: File | null = null;

  // Oluşturulan kullanıcının ID’si
  createdPersonId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private familyPersonService: FamilyPersonService,
    private roleService:RoleService
  ) {}

  ngOnInit(): void {
    this.getFamilyPersons()
    this.getRoles()
    this.initializePersonForm();
  }

  /**
   * Kişi ekleme (fotoğrafsız) formunu oluşturma
   */
  initializePersonForm() {
    this.personForm = this.fb.group({
      fullName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
      profilePicture:["",Validators.required],
      roleId: [0, Validators.required]
    });
  }

  /**
   * Kişi ekleme formu submit
   */
  onPersonFormSubmit() {
    if (this.personForm.valid) {
      // Form değerlerini al
      const personData: FamilyPerson = {
        id: 0,  // Yeni eklenen kişi
        fullName: this.personForm.value.fullName,
        email: this.personForm.value.email,
        password: this.personForm.value.password,
        profilePicture: '', // başlangıçta boş
        roleId: this.personForm.value.roleId
      };

      // API çağrısı: kişi ekle
      this.familyPersonService.addFamilyPerson(personData).subscribe({
        next: (createdPerson) => {
          console.log('Kişi eklendi:', createdPerson);
          // Yeni oluşturulan kaydın Id'sini saklıyoruz
          this.createdPersonId = createdPerson.id;
          alert('Kişi başarıyla eklendi. Eğer fotoğraf eklemek isterseniz aşağıdan yükleyebilirsiniz.');
        },
        error: (err) => {
          console.error('Kişi ekleme hatası:', err);
          alert('Kişi eklerken hata oluştu!');
        }
      });
    } else {
      alert('Lütfen tüm gerekli alanları doldurun.');
    }
  }

 
  getFamilyPersons(){
    this.familyPersonService.getFamilyPersons().subscribe(response=>{
      this.familyPersons=response.data
    })
  }
  getRoles(){
    this.roleService.getRoles().subscribe(response=>{
      this.roles=response.data
    })
  }
}
------------------------
ayrıca tüm familyPerson işlemlerini yapmamızı sağlayacak familyPerson servisini(Fotoğraf ekleme dahil) oluşturuyoruz
------------------------
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ListResponseModel } from '../models/listResponseModel';
import { FamilyPerson } from '../models/familyPerson';

@Injectable({
  providedIn: 'root'
})
export class FamilyPersonService {
  id: number;
  fullName: string;
  email: string;
  password: string;
  profilePicture: string;
  roleId: number;
  apiUrl = 'https://localhost:7039/api/';


  constructor(private httpClient:HttpClient) { }

  getFamilyPersons():Observable<ListResponseModel<FamilyPerson>>{
    let newPath=this.apiUrl+"familypersons/getall"
    return this.httpClient.get<ListResponseModel<FamilyPerson>>(newPath)
  }

  addFamilyPerson(person: FamilyPerson): Observable<FamilyPerson> {
    // Ör: POST /api/familypersons/addfamilyperson
    return this.httpClient.post<FamilyPerson>(`${this.apiUrl}familyPersons/addfamilyperson`, person);
  }

  uploadProfilePicture(familyPersonId: number, file: File): Observable<any> {
    const formData = new FormData();
    formData.append('profilePicture', file);
    // Ör: POST /api/familypersons/upload-profile-picture/{familyPersonId}
    return this.httpClient.post(`${this.apiUrl}familypersons/upload-profile-picture/${familyPersonId}`, formData);
  }
}
---------------------------
öncelikle familyPerson.Html dosyasını düzenliyoruz. Bu sayfada amaç kişi listesini göstermek ve altta yeni kişi eklenmesini sağlayan form olacak ayrıca buraya ek bir buton koyarak var olan kişiye fotoğraf ekleme sayfasına gidilmesi sağlanacak.
-------------------------
<table class="table">
  <thead>
    <tr>
      <th>Family Id</th>
      <th>Adı Soyadı</th>
      <th>Email</th>
      <th>Profil Resmi</th>
    </tr>
  </thead>

  <tr *ngFor="let familyPerson of familyPersons">
    <td>{{ familyPerson.id }}</td>
    <td>{{ familyPerson.fullName }}</td>
    <td>{{ familyPerson.email }}</td>
    <td>
      <img
        src="{{ familyPerson.profilePicture }}"
        class="img-thumbnail"
        alt="..."
      />
    </td>
  </tr>
</table>
<div class="container">
  <h2>FamilyPerson Oluşturma</h2>
  <form [formGroup]="personForm" (ngSubmit)="onPersonFormSubmit()">
    <div>
      <label for="fullName">Full Name:</label>
      <input
        id="fullName"
        class="form-control"
        formControlName="fullName"
        type="text"
        placeholder="Ad Soyad"
      />
      <div *ngIf="personForm.get('fullName')?.invalid && personForm.get('fullName')?.touched">
        <small style="color: red;">Zorunlu alan.</small>
      </div>
    </div>
    <div>
      <label for="email">Email:</label>
      <input
        id="email"
        class="form-control"
        formControlName="email"
        type="email"
        placeholder="Email"
      />
      <div *ngIf="personForm.get('email')?.invalid && personForm.get('email')?.touched">
        <small style="color: red;">Geçerli bir email giriniz.</small>
      </div>
    </div>
    <div>      
      <input
        id="password"
        class="form-control"
        formControlName="password"
        type="password"
        placeholder="Şifre"
      />
      <div *ngIf="personForm.get('password')?.invalid && personForm.get('password')?.touched">
        <small style="color: red;">Zorunlu alan.</small>
      </div>
    </div>
    <div class="mb-3">
      <label for="roleId"><h6>Rol</h6></label>
      <select
        class="form-select"
        aria-label="Default select example"
        id="roleId"
        formControlName="roleId"             
      >
      <option selected>
       Rol seçiniz
      </option>
        <option *ngFor="let role of roles" [ngValue]="">
          {{ role.name }}
        </option>
      </select>
    </div>
<div>
  <button class="btn btn-success" type="submit" [disabled]="personForm.invalid">Kişiyi Ekle (Fotoğrafsız)</button>
</div>  

  </form>

  <hr />
  <div>
    <button class="btn btn-success" type="submit" routerLink="/familypersons/upload-profile-picture/familyPersonId" >Var olan kişiye profil fotoğrafı ata</button>
  </div>
  
 
</div>
----------------------------
Burada var olan bir kişiye fotoğraf eklemek için upload-profile-picture adında yeni bir component oluşturuyoruz.
---------------------------
import { Component, OnInit } from '@angular/core';
import { FamilyPersonService } from '../../services/family-person.service';
import { ToastrService } from 'ngx-toastr';
import { FamilyPerson } from '../../models/familyPerson';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-upload-profile-picture',
  templateUrl: './upload-profile-picture.component.html',
  standalone: false,
})
export class UploadProfilePictureComponent implements OnInit {
  familyPersonId: number | null = null; // Kullanıcı ID'si
  selectedFile: File | null = null; // Seçilen fotoğraf
  familyPersons:FamilyPerson[]
  photoForm!: FormGroup;

  constructor(
    private familyPersonService: FamilyPersonService,
    private toastrService: ToastrService,
    private fb:FormBuilder
  ) {}
  ngOnInit(): void {
    this.getFamilyPersons()
    this.initializeForm()
  }

  initializeForm(): void {
    this.photoForm = this.fb.group({
      // Kişi seçimi
      selectedPersonId: [null, Validators.required],
      // Fotoğraf input - reaktif form kontrol olmadan da handle edebilirsiniz; 
      // ama input type="file" reaktif formla da çalışabilir.
    });
  }

  /**
   * Dosya seçildiğinde tetiklenir
   */
  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
    }
  }

  getFamilyPersons() {
    this.familyPersonService.getFamilyPersons().subscribe((response) => {
      this.familyPersons=response.data
    });
  }

  /**
   * Form submit
   */
  onSubmit(): void {
    if (this.photoForm.invalid) {
      alert('Lütfen bir kişi seçiniz ve fotoğraf ekleyiniz.');
      return;
    }
    if (!this.selectedFile) {
      alert('Fotoğraf seçilmedi.');
      return;
    }
    const familyPersonId = this.photoForm.value.selectedPersonId;
    this.familyPersonService.uploadProfilePicture(familyPersonId, this.selectedFile).subscribe({
      next: (res) => {
        console.log('Fotoğraf yüklendi:', res);
        this.toastrService.success("Kişiye profil resmi eklendi")
      },
      error: (err) => {
        console.error('Fotoğraf yükleme hatası:', err);
        this.toastrService.success("Kişiye profil resmi eklendi")
      }
    });
  }
}
-------------------------
html dosyası da şu şekilde olacak
--------------------------
<div class="container">
    <h2>Kişiye Fotoğraf Ekle</h2>  
    <form [formGroup]="photoForm" (ngSubmit)="onSubmit()">
      <!-- Kişi seçimi (select) -->
      <div>
        <label class="form-control" for="personSelect">FamilyPerson Seç:</label>
        <select class="form-select" id="personSelect" formControlName="selectedPersonId">
          <option [ngValue]="null" disabled>-- Seçiniz --</option>
          <option *ngFor="let fp of familyPersons" [value]="fp.id">
            {{ fp.fullName }} (ID: {{fp.id}})
          </option>
        </select>  
        <!-- Hata gösterimi -->
        <div *ngIf="photoForm.get('selectedPersonId')?.invalid && photoForm.get('selectedPersonId')?.touched">
          <small style="color: red;">Lütfen bir kişi seçiniz.</small>
        </div>
      </div>  
      <!-- Fotoğraf seçme alanı -->
      <div>
        <label class="form-control" for="photoFile">Profil Fotoğrafı:</label>
        <input class="form-control" id="photoFile" type="file" (change)="onFileSelected($event)" />
      </div>  
      <!-- Formu gönder -->
      <button class="btn btn-success" type="submit" [disabled]="photoForm.invalid">Fotoğraf Yükle</button>
    </form>
  </div>
  ---------------------------
 
  Şİmdi de familyPerson.component.html dosyasında familyPerson ların listelendiği yerde thumbnail fotoğrafların görülebilmesi için API'den gelen fotoğraf adresinin [src] içinde tam adresi yazıyoruz ve böylece fotoğraflar görülüyor.
  ----------------------------
  <table class="table">
  <thead>
    <tr>
      <th>Family Id</th>
      <th>Adı Soyadı</th>
      <th>Email</th>
      <th>Profil Resmi</th>
    </tr>
  </thead>

  <tr *ngFor="let familyPerson of familyPersons">
    <td>{{ familyPerson.id }}</td>
    <td>{{ familyPerson.fullName }}</td>
    <td>{{ familyPerson.email }}</td>
    <td>
      <img [src]="'https://localhost:7039/uploads/profile_pictures/' + familyPerson.profilePicture"
      class="img-thumbnail" alt="Profil Resmi" style="width: 100px; height: auto;" />
    </td>
  </tr>
</table>
---------------------------
Kullandığımız route ları da app.routing.module şu şekilde ekliyoruz.
------------------------
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ExpenseComponent } from './components/expense/expense.component';
import { FamilyPersonComponent } from './components/family-person/family-person.component';
import { CategoryComponent } from './components/category/category.component';
import { IncomeComponent } from './components/income/income.component';
import { MarketItemComponent } from './components/market-item/market-item.component';
import { CategoryAddComponent } from './components/category-add/category-add.component';
import { UnitAddComponent } from './components/unit-add/unit-add.component';
import { UnitComponent } from './components/unit/unit.component';
import { RoleComponent } from './components/role/role.component';
import { RoleAddComponent } from './components/role-add/role-add.component';
import { UploadProfilePictureComponent } from './components/upload-profile-picture/upload-profile-picture.component';

const routes: Routes = [
  { path: '', component: ExpenseComponent },
  { path: 'expenses', component: ExpenseComponent },
  { path: 'familypersons', component: FamilyPersonComponent },
  { path: 'categories', component: CategoryComponent },
  { path: 'incomes', component: IncomeComponent },
  { path: 'marketitems', component: MarketItemComponent },
  { path: 'expenses/category/:id', component: ExpenseComponent },
  { path: 'categories/addcategory', component: CategoryAddComponent },
  { path: 'units/getall', component: UnitComponent },
  { path: 'units/addunit', component: UnitAddComponent },
  { path: 'roles', component: RoleComponent },
  { path: 'roles/addrole', component: RoleAddComponent },
  {path:"familypersons/upload-profile-picture/familyPersonId",component:UploadProfilePictureComponent},
  {path:"familypersons/addfamilyperson",component:FamilyPersonComponent} 
 
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
-------------------------------
   (Burada kaldık)

   REGİSTER-LOGİN-YETKİLENDİRME İŞLEMLERİ
   -------------------------------------------
componentin içine girerek ng g component login ile login adında bir component oluşturuyoruz. Bundan sonra bootstrap sayafsında examples kısmında sign-in kısmını açıyoruz.https://getbootstrap.com/docs/5.3/examples/sign-in/ bu sayfadaki kodları almak için sayfaya sağ tıklıyoruz. sayfa kaynağını görüntüle ye basıyoruz. oradan kodu şu şekilde alıyoruz.
-----------------------------
<body>
<main class="form-signin w-100 m-auto">
  <form>
    <img class="mb-4" src="/docs/5.3/assets/brand/bootstrap-logo.svg" alt="" width="72" height="57">
    <h1 class="h3 mb-3 fw-normal">Please sign in</h1>
    <div class="form-floating">
      <input type="email" class="form-control" id="floatingInput" placeholder="name@example.com">
      <label for="floatingInput">Email address</label>
    </div>
    <div class="form-floating">
      <input type="password" class="form-control" id="floatingPassword" placeholder="Password">
      <label for="floatingPassword">Password</label>
    </div>
    <div class="form-check text-start my-3">
      <input class="form-check-input" type="checkbox" value="remember-me" id="flexCheckDefault">
      <label class="form-check-label" for="flexCheckDefault">
        Remember me
      </label>
    </div>
    <button class="btn btn-primary w-100 py-2" type="submit">Sign in</button>
    <p class="mt-5 mb-3 text-body-secondary">&copy; 2017–2024</p>
  </form>
</main>
<script src="/docs/5.3/dist/js/bootstrap.bundle.min.js" integrity="sha384-YvpcrYf0tY3lHB60NNkmXc5s9fDVZLESaAA55NDzOxhy9GkcIdslK1eN7N6jIeHz" crossorigin="anonymous"></script>
</body>
----------------------------
Ancak görüntümüz bizim aldığımız sayfadaki gibi görünmüyor bu sebeple sayfamızın sayfa kaynağına geri dönüyoruz ve orada kullanılan sign-inn.css 'inin üstüne tıklayarak css 'i görüntülüyoruz ve css'i kopyalayıp projemizdeki login.css dosyamıza kopyalıyoruz. logonun adresini de kopyalayınca görüntümüz düzeliyor. https://getbootstrap.com/docs/5.3/assets/brand/bootstrap-logo.svg
-----------------------------
şimdi bu sayfayı kullanacak componentimizde işlem yapmaya geldi html sayfasındaki formu mapleyecek loginForm adında bir yapımız olacak.
ama önce auth service'imizi oluşturalım. services klasörü içine girerek ng g service auth komutuyla auth servisimizi oluşturuyoruz. ama bize login olma sırasında hangi bilgiler geiyor ona bakmalıyız burada bize token bilgisi ve expiration bilgisi geliyor. Buna angularda tokenModel.ts adında bir model oluşturuyoruz. models üzerinde sağ tıklıyoruz ve yeni dosya oluştura basarak tokenModel.ts ve loginModel.ts dosyalarını oluşturuyoruz. 
-----------------------
export interface TokenModel{
    token:string
    expiration:string
}
--------------------------
export interface LoginModel{
    email:string
    password:string
}
-------------------------
Bizde API'den gelen bilgiler Data bilgisi, isSuccess bilgisi ve mesaj bilgisi ama bize birde Token bilgisi gelmesi gerekiyor bu sebeple bazı değişiklikler yapmamız gerekiyor yani kodlarımızı refaktör ediyoruz. öncelikle models klasöründe singleResponseModel.ts adında yeni bir dosya oluşturuyoruz. Burada içinde token bilgisi ve expiration olan tek bir data dönüyor bir data array bilgisi dönmediğinden listResponseModel.ts bizim isteğimizi karşılamıyor. ama token ve expiration bilgisinin yanında isSuccess ve Message bilgisininde dönmesini istediğimizden responseModel.ts dosyası ile extends ediyoruz.
----------------------------
import { ResponseModel } from "./responseModel";

export interface SingleResponseModel<T> extends ResponseModel{
    data:T
}
-------------------------
auth.service şu şekilde oluşturuyoruz.
-------------------------
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LoginModel } from '../models/loginModel';
import { TokenModel } from '../models/tokenModel';
import { SingleResponseModel } from '../models/singleResponseModel';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  apiUrl = 'https://localhost:7206/api/auth/';
  constructor(private httpClient:HttpClient) { }

  login(loginModel:LoginModel){
    return this.httpClient.post<SingleResponseModel<TokenModel>>(this.apiUrl+"login",loginModel)
  }

  isAuthenticated(){
    if(localStorage.getItem("token")){
      return true
    }else{
      return false
    }
  }
}
--------------------------
login.component.ts dosyası şu şekilde oluyor.
-------------------------
import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormControl,
  Validators,
} from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-login',
  standalone: false,

  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private toastrService: ToastrService
  ) {}

  ngOnInit(): void {
    this.createLoginForm();
  }

  createLoginForm() {
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required]],
      password: ['', Validators.required],
    });
  }

  login() {
    if (this.loginForm.valid) {
      let loginModel = Object.assign({}, this.loginForm.value);
      this.authService.login(loginModel).subscribe((response) => {
        this.toastrService.success(response.message)    
        localStorage.setItem("token",response.data.token)    
      },responseError=>{
        console.log(responseError)
        this.toastrService.error(responseError.error)
      });
    }
  }
}
------------------------
Böylece login yapılırken localStorage içinde token oluşuyor. Şimdi bizim yetkilendirmelerimiz vardı veritabanına baktığımızda kullanıcımızın product.add yetkisinin olduğunu görüyoruz. angularda product add sırasında tokenide de API'ye göndermek gerekiyor bu sebeple şöyle düzenleme yapıyoruz. bunu product service teki add metodunda ürünü gönderirken headers altında tek tek gönderebiliriz ama biz her metot için tek tek göndermek yerine çalışan her metotla birlikte token bilgisini de göndereceğimiz aspect şeklinde yapacağız. Bu sebeple app clasörüne sağ tıklıyoruz ve interceptors adında yeni bir klasör açıyoruz. open in integrated terminal ile içine girin ve ng g interceptor auth ile bir interceptor oluşturuyoruz.
----------------------------
import { HttpInterceptorFn, HttpRequest } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {

  let token=localStorage.getItem("token")
  let newRequest:HttpRequest<any>
  newRequest=req.clone({
    headers:req.headers.set("Authorization","Bearer "+token)
  })
  return next(newRequest);
};
------------------------
Burada gelen requeste token bilgisinide ekleyerek cevap veriyoruz. Bunun devreye girebilmesi için app.module.ts dosyasına şu eklemeyi yapıyoruz daha önceden useClass kullanılıyordu ancak yeni angularda useValue kullanmak gerekiyor(provider[] kısmına)
-------------------------
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';


import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ProductComponent } from './components/product/product.component';
import { CategoryComponent } from './components/category/category.component';
import { NaviComponent } from './components/navi/navi.component';
import { HTTP_INTERCEPTORS, provideHttpClient } from '@angular/common/http';
import { VatAddedPipe } from './pipes/vat-added.pipe';
import { FormsModule,ReactiveFormsModule } from '@angular/forms';
import { FilterPipePipe } from './pipes/filter-pipe.pipe';
import { ToastrModule } from 'ngx-toastr';
import { CartSummaryComponent } from './components/cart-summary/cart-summary.component';
import { ProductAddComponent } from './components/product-add/product-add.component';
import { LoginComponent } from './components/login/login.component';
import { AuthInterceptor } from './interceptors/auth.interceptor';


@NgModule({
  declarations: [
    AppComponent,
    ProductComponent,
    CategoryComponent,
    NaviComponent,
    VatAddedPipe,
    FilterPipePipe,
    CartSummaryComponent,
    ProductAddComponent,
    LoginComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    ToastrModule.forRoot({
      positionClass:"toast-bottom-right",

    }),
    BrowserAnimationsModule
    
  ],
  providers: [
    provideHttpClient(),
    {
      provide: HTTP_INTERCEPTORS,
      useValue: AuthInterceptor,  // Fonksiyon tabanlı interceptor
      multi: true
    }
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }
----------------------------
şimdi artık önce token yokken bir ürün eklemeye çalışalim f12 ye basarak oradan application içinde localstorage içinde sağ tıklayıp clear diyerek tokeni siliyoruz ve ürün eklemeye çalışıyoruz. ve yetkiniz yok uyarısı alıyoruz şimdi login olup öyle deneyelim.Yeni angularda interceptora girmediğinden yine yetki hatası verdi chatgpt'ye sorarak app.module ve interceptors'de bazı değişiklikler yaptım. şu şekilde değişti kodlar
------------------------------
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';


import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ProductComponent } from './components/product/product.component';
import { CategoryComponent } from './components/category/category.component';
import { NaviComponent } from './components/navi/navi.component';
import { HTTP_INTERCEPTORS, provideHttpClient,withInterceptors} from '@angular/common/http';
import { VatAddedPipe } from './pipes/vat-added.pipe';
import { FormsModule,ReactiveFormsModule } from '@angular/forms';
import { FilterPipePipe } from './pipes/filter-pipe.pipe';
import { ToastrModule } from 'ngx-toastr';
import { CartSummaryComponent } from './components/cart-summary/cart-summary.component';
import { ProductAddComponent } from './components/product-add/product-add.component';
import { LoginComponent } from './components/login/login.component';
import { AuthInterceptor } from './interceptors/auth.interceptor';


@NgModule({
  declarations: [
    AppComponent,
    ProductComponent,
    CategoryComponent,
    NaviComponent,
    VatAddedPipe,
    FilterPipePipe,
    CartSummaryComponent,
    ProductAddComponent,
    LoginComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    ToastrModule.forRoot({
      positionClass:"toast-bottom-right",

    }),
    BrowserAnimationsModule
    
  ],
  providers: [
    provideHttpClient(),
    provideHttpClient(
      withInterceptors([AuthInterceptor])
    ),
    {
      provide: HTTP_INTERCEPTORS,
      useValue: AuthInterceptor,  // Fonksiyon tabanlı interceptor
      multi: true
    }
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }
------------------------------
import { HttpInterceptorFn, HttpRequest } from '@angular/common/http';

export const AuthInterceptor: HttpInterceptorFn = (req, next) => {
  // localStorage'dan token'ı alıyoruz
  const token = localStorage.getItem('token');

  // İsteği klonlayarak Authorization başlığını ekliyoruz
  const authReq: HttpRequest<any> = req.clone({
    setHeaders: {
      Authorization: `Bearer ${token ?? ''}`
    }
  });

  // Gerekirse debug için log atabilirsiniz
  console.log('AuthInterceptor - Request:', authReq);

  // Zincirdeki bir sonraki handle'a klonlanmış isteği gönderiyoruz
  return next(authReq);
};
--------------------------------
 add() {
    if (this.productAddForm.valid) {
      // Form verilerini model nesnesine kopyalıyoruz
      let productModel = Object.assign({}, this.productAddForm.value);  
      this.productService.add(productModel).subscribe(
        (response) => {
          // Backend'den dönen veriyi tekrar productModel'e atıyoruz
          productModel = response;
          // Başarılı işlem mesajı
          this.toastrService.success(
            'Bir ürün eklendi',
            productModel.productName
          );
        },
        (responseError) => {
          // Hata nesnesi varsa ve ValidationErrors bir dizi ise
          if (
            responseError.error?.ValidationErrors &&
            Array.isArray(responseError.error.ValidationErrors) &&
            responseError.error.ValidationErrors.length > 0
          ) {
            for (
              let i = 0;
              i < responseError.error.ValidationErrors.length;
              i++
            ) {
              this.toastrService.error(
                responseError.error.ValidationErrors[i].ErrorMessage,
                'Doğrulama Hatası'
              );
            }
          } else {
            // Beklenmeyen bir hata durumunu yönetmek için genel bir mesaj
            this.toastrService.error(
              'Ürün ekleme sırasında beklenmeyen bir hata oluştu.',
              'Hata'
            );
          }
        }
      );
    } else {
      this.toastrService.error(
        'Ürün ekleme formu geçersiz, lütfen alanları kontrol ediniz.',
        'Hata'
      );
    }
  }
----------------------------
Şimdi ise yapmak istediğimiz şey Eğer login olmamışsa sayfalara ulaşamasın ve direct login ekranına yönlendirilsin app ye bir klasör daha oluşturuyoruz ismi guards olacak
------------------------------
// login.guard.ts
import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { Router } from '@angular/router';

export const loginGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const token = localStorage.getItem('token');

  // Eğer token varsa erişime izin ver
  if (token) {
    return true; 
  }

  // Token yoksa /login sayfasına yönlendir
  return router.createUrlTree(['/login']);
};
--------------------------
app.route.da bunu kullanacak sayfalar ayarlanıyor.
--------------------------
  {path:"products/add",component:ProductAddComponent,canActivate:[loginGuard]},
-------------------------
böylece token yok ise products/add sayfasını getirmeye çalıştığımızda bizi direk olarak login sayfasına gönderir.

   




















    

  
