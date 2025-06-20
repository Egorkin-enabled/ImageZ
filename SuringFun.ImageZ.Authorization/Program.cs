using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SuringFun.ImageZ.Authorization.Controller;
using static SuringFun.ImageZ.Essentials.EnvironmentHelper;
using static SuringFun.ImageZ.Essentials.DbConsts;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.
    AddIdentityCore<IdentityUser>().
    AddEntityFrameworkStores<IdentityDbContext>();
builder.Services.AddDbContext<IdentityDbContext>(
    // x => x.UseNpgsql(
    //     ChallengeEnv(builder.Configuration, EnvDbConStr)
    // )
    x => x.UseInMemoryDatabase("Auth")
);

builder.Services.AddIdentityCore<IdentityUser>();
builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();
