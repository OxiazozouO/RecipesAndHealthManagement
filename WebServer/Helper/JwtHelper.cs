using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebServer.Configurations;

namespace WebServer.Helper;

public enum JwtType
{
    User,
    Admin
}

public class JwtHelper(JwtConfig jwtConfig)
{
    private JwtConfig JwtConfig { get; set; } = jwtConfig;

    public static void AddJwtService(JwtConfig jwtConfig, IServiceCollection services)
    {
        new JwtHelper(jwtConfig).AddJwtService(services);

        services.AddSwaggerGen(options =>
        {
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        }
                    },
                    Array.Empty<string>()
                }
            });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT授权(数据将在请求头中进行传输) 在下方输入Bearer {token} 即可，注意两者之间有空格",
                Name = "Authorization", //jwt默认的参数名称
                In = ParameterLocation.Header, //jwt默认存放Authorization信息的位置(请求头中)
                Type = SecuritySchemeType.ApiKey,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
        });
    }


    /// <summary>
    /// 添加Jwt服务
    /// </summary>
    public void AddJwtService(IServiceCollection services)
    {
        services.AddSingleton(this)
            .AddAuthentication(option =>//认证middleware配置
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //Token颁发机构
                    ValidIssuer = JwtConfig.Issuer,
                    //颁发给谁
                    ValidAudience = JwtConfig.Audience,
                    
                    IssuerSigningKey = JwtConfig.SymmetricSecurityKey,//这里的key要进行加密
                    //是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = true,
                };
            });
    }

    /// <summary>
    /// 添加claims信息
    /// </summary>
    /// <param name="claims"></param>
    /// <returns></returns>
    public string GetJwtToken(List<Claim> claims)
    {
        var jwtSecurityToken = new JwtSecurityToken(
            JwtConfig.Issuer,
            JwtConfig.Audience,
            claims,
            JwtConfig.NotBefore,
            JwtConfig.Expiration,
            JwtConfig.SigningCredentials
        );
        return "Bearer " + new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }


    /// <summary>
    /// 获取jwt token
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="userName"></param>
    /// <param name="userType"></param>
    /// <returns></returns>
    public string GetJwtToken(long userId, string userName, JwtType userType)
    {
        var claims = new List<Claim>
        {
            new("UserId", userId.ToString()),
            new("UserName", userName),
            new(ClaimTypes.Role, userType.ToString()),
        };
        return GetJwtToken(claims);
    }

    /// <summary>
    /// 获取Jwt的信息
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public IEnumerable<Claim> Decode(HttpRequest request)
    {
        var auth = request.Headers["Authorization"].ToString().Split(" ")[1];
        return new JwtSecurityTokenHandler().ReadJwtToken(auth).Payload.Claims;
    }

    /// <summary>
    /// 解析得到User
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public (string UserId, string UserName, string UserType) DecodeToUser(HttpRequest request)
    {
        var claims = Decode(request);
        var enumerable = claims as Claim[] ?? claims.ToArray();
        return (
            enumerable.First(t => t.Type == "UserId").Value,
            enumerable.First(t => t.Type == "UserName").Value,
            enumerable.First(t => t.Type == ClaimTypes.Role).Value
        );
    }
}