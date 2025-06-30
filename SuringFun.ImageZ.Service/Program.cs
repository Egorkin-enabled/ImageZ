using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using SuringFun.ImageZ.Service.Model;
using static SuringFun.ImageZ.Essentials.JwtConsts;
using static SuringFun.ImageZ.Essentials.EnvironmentHelper;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SuringFun.ImageZ.Service.Service.RandomServices;
using SuringFun.ImageZ.Service.Service.Databases;
using SuringFun.ImageZ.Service.Service.FileServices;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.
    AddIdentityCore<Author>().
    AddEntityFrameworkStores<ServiceDbContext>();
builder.Services.AddDbContext<ServiceDbContext>(
    // x => x.UseNpgsql(
    //     "Host=localhost:5432;Username=postgres;Password=123;Database=testdb"
    // )
    x => x.UseInMemoryDatabase("Serivce")
);

builder.Services.AddCryptoRandom();
builder.Services.AddInMemoryFileService();

builder.Services.AddControllers();

builder.Services.
    AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
    AddJwtBearer(
        x =>
        {
            x.Audience =
                ChallengeEnv(
                    builder.Configuration,
                    EnvJwtAudience
                    );
            x.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidIssuer = ChallengeEnv(
                    builder.Configuration,
                    EnvJwtIssuer
                ),
                ValidateAudience = true,
                ValidAudience = ChallengeEnv(
                    builder.Configuration,
                    EnvJwtAudience
                ),
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        ChallengeEnv(
                            builder.Configuration,
                            EnvJwtSecret
                        )
                    )
                )
            };
        }
    );
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
