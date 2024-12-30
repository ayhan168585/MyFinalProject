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
 














    

  
