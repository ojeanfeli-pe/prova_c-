using Matheus.Modelos;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDataContext>();
var app = builder.Build();

app.MapGet("/", () => "Prova em Dupla com consulta!");

app.MapPost("/api/funcionario/cadastrar", ([FromBody] Funcionario funcionario,[FromServices] AppDataContext ctx) =>
{
    ctx.Funcionarios.Add(funcionario);
    ctx.SaveChanges();
    return Results.Created($"/produto/{funcionario.Id}", funcionario);
});

app.MapGet("/api/funcionario/listar", ([FromServices] AppDataContext ctx) =>
{
        return Results.Ok(ctx.Funcionarios.ToList());
});

app.MapPost("/api/folha/cadastrar", ([FromBody] Folha folha,[FromServices] AppDataContext ctx) =>
{
    //CALCULAR O SALARIO BRUTO
    folha.SalarioBruto = folha.Quantidade * folha.Valor;

    //CALCULAR O IRRF
    if(folha.SalarioBruto <= 1903.98)
        folha.ImpostoIRRF = 0;
    if(folha.SalarioBruto <= 2826.65)
        folha.ImpostoIRRF = (folha.SalarioBruto * .075) - 142.80;
    if(folha.SalarioBruto <= 3751.05)
        folha.ImpostoIRRF = (folha.SalarioBruto * .15) - 354.80;
    if(folha.SalarioBruto <= 4664.68)
        folha.ImpostoIRRF = (folha.SalarioBruto * .225) - 636.13;
    else
        folha.ImpostoIRRF = (folha.SalarioBruto * .275) - 869.36;

    //CALCULAR O INSS
    if(folha.SalarioBruto <= 1693.72)
        folha.ImpostoINSS = folha.SalarioBruto * .08;
    if(folha.SalarioBruto <= 2822.90)
        folha.ImpostoINSS = folha.SalarioBruto * .09;
    if(folha.SalarioBruto <= 5645.80)
        folha.ImpostoINSS = folha.SalarioBruto * .11;
    else
        folha.ImpostoINSS = 621.04;

    //CALCULAR O FGTS
    folha.ImpostoFGTS = folha.SalarioBruto * .08;

    //CALCULAR O SALÁRIO LÍQUIDO
    folha.SalarioLiquido = folha.SalarioBruto - folha.ImpostoIRRF - folha.ImpostoINSS;

    ctx.Folhas.Add(folha);
    ctx.SaveChanges();
    return Results.Created($"/produto/{folha.Id}", folha);
});

app.MapGet("/api/folha/listar", ([FromServices] AppDataContext ctx) =>
{
        return Results.Ok(ctx.Folhas.ToList());
});

app.MapGet("/api/folha/buscar/{mes}/{ano}", ([FromServices] AppDataContext ctx, [FromRoute] int mes, [FromRoute] int ano) =>
{
    Folha? folha = ctx.Folhas.FirstOrDefault(f => f.Mes == mes && f.Ano == ano);
    if(folha is null){
        return Results.NotFound();
    }
    return Results.Ok(folha);
});
app.Run();
