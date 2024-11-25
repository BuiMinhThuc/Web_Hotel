using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QLKS_v1.DataContext;
using QLKS_v1.Entities;
using QLKS_v1.Implements;
using QLKS_v1.Interfaces;
using QLKS_v1.Payload.Converters;
using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Responses;
using QLKS_v1.Services.Implements;
using QLKS_v1.Services.Interfaces;
using System.Text;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyPolicy",
        policy =>
        {
            policy.WithOrigins("*")
                    .AllowAnyMethod().AllowAnyHeader();
        });
});







builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Swagger eShop Solution", Version = "v1" });
    x.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Làm theo mẫu này. Example: Bearer {Token} ",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    x.AddSecurityRequirement(new OpenApiSecurityRequirement()
                  {
                    {
                      new OpenApiSecurityScheme
                      {
                        Reference = new OpenApiReference
                          {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                          },
                          Scheme = "oauth2",
                          Name = "Bearer",
                          In = ParameterLocation.Header,
                        },
                        new List<string>()
                      }
                    });
    //x.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateAudience = false,
        ValidateIssuer = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            builder.Configuration.GetSection("AppSettings:SecretKey").Value!))
    };
});
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
    options.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
});

builder.Services.AddScoped<IService_User, Service_User>();
builder.Services.AddScoped<ResponseObject<DTO_Token>>();
builder.Services.AddScoped<IService_Booking, Service_Booking>();

builder.Services.AddScoped<Converter_Booking>();
builder.Services.AddScoped<Converter_BookingService>();
builder.Services.AddScoped<Converter_Customer>();
builder.Services.AddScoped<Converter_Service>();
builder.Services.AddScoped<IService_Room, Service_Room>();

builder.Services.AddScoped<IService_Bill, Service_Bill>();
builder.Services.AddScoped<IService_Customer, Service_Customer>();
builder.Services.AddScoped<IService_RoomType, Service_RoomType>();
builder.Services.AddScoped<IService_Staff, Service_Staff>();
builder.Services.AddScoped<IService_Promotion, Service_Promotion>();
builder.Services.AddScoped<IService_FeedBackCustomer, Service_FeedBackCustomer>();
builder.Services.AddScoped<IService_StaffShift, Service_StaffShift>();
builder.Services.AddScoped<IService_TypeService, Service_TypeService>();
builder.Services.AddScoped<IService_Equiment, Service_Equiment>();
builder.Services.AddScoped<IService_RoomImage, Service_RoomImage>();
builder.Services.AddScoped<IService_Service, Service_Service>();
builder.Services.AddScoped<IService_BookingService, Service_BookingService>();
builder.Services.AddScoped<IVNPayService, VNPayService>();
builder.Services.AddScoped<IService_News, Service_News>();
builder.Services.AddScoped<Converter_ChooseRoom>();
builder.Services.AddScoped<Converter_Staff>();
builder.Services.AddScoped<Converter_News>();
builder.Services.AddScoped<Converter_TypeService>();
builder.Services.AddScoped<Converter_CustomerFeedBack>();
builder.Services.AddScoped<Converter_StaffShift>();
builder.Services.AddScoped<Converter_ChoonseService>();
builder.Services.AddScoped<Converter_Bill>();
builder.Services.AddScoped<Converter_Room>();
builder.Services.AddScoped<Converter_Equitment>();
builder.Services.AddScoped<DTO_Room>();
builder.Services.AddScoped<DTO_Staff>();
builder.Services.AddScoped<DTO_TypeRoomHot>();
builder.Services.AddScoped<DTO_BillServiceForPDF>();
builder.Services.AddScoped<DTO_Service>();
builder.Services.AddScoped<DTO_BillFullRoom>();
builder.Services.AddScoped<DTO_RoomHot>();
builder.Services.AddScoped<DTO_ViewChiTietRoom>();
builder.Services.AddScoped<DTO_ImageRoom>();
builder.Services.AddScoped<Converter_ImageRoom>();
builder.Services.AddScoped<Converter_Equitment>();
builder.Services.AddScoped<Converter_getRoomTypeName>();
builder.Services.AddScoped<ResponseBase>();
builder.Services.AddScoped<Converter_User>();
builder.Services.AddScoped<ResponseObject<DTO_GetUser>>();
builder.Services.AddScoped<ResponseObject<DTO_Booking>>();
builder.Services.AddScoped<DTO_Booking>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("MyPolicy");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
