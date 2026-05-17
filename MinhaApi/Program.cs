using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// ───────────────────────────────
// PROGRAM
// ───────────────────────────────
//RM7MD1ZJV736NFSCQ141GA5T
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=orcamentos.db")); // ✅ cria o arquivo .db automaticamente
builder.Services.AddScoped<OrcamentoService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

// ✅ Cria o banco automaticamente se não existir
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseRouting();
app.MapControllers();

app.Run();

// ───────────────────────────────
// MODEL
// ───────────────────────────────
public class Orcamento
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? TipoImovel { get; set; }
    public string? Cep { get; set; }        // ✅ novo
    public string? Cidade { get; set; }
    public string? CidadeNome { get; set; } // ✅ novo
    public string? Urgencia { get; set; }
    public string? Mensagem { get; set; }
    public string Servicos { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; } = DateTime.Now;
}
// ───────────────────────────────
// DB CONTEXT
// ───────────────────────────────
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Orcamento> Orcamentos { get; set; }
}

// ───────────────────────────────
// SERVICE
// ───────────────────────────────
public class OrcamentoService
{
    private readonly AppDbContext _db;

    public OrcamentoService(AppDbContext db)
    {
        _db = db;
    }

    public void Adicionar(Orcamento o)
    {
        _db.Orcamentos.Add(o);
        _db.SaveChanges(); // ✅ salva no banco
    }

    public List<Orcamento> Listar()
    {
        return _db.Orcamentos.ToList();
    }
}

// ───────────────────────────────
// CONTROLLER
// ───────────────────────────────
[ApiController]
[Route("api/[controller]")]
public class OrcamentosController : ControllerBase
{
    private readonly OrcamentoService _service;

    public OrcamentosController(OrcamentoService service)
    {
        _service = service;
    }

    [HttpPost]
    public IActionResult Criar([FromBody] OrcamentoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            return BadRequest(new { erro = "Nome é obrigatório" });

        if (string.IsNullOrWhiteSpace(request.Telefone))
            return BadRequest(new { erro = "Telefone é obrigatório" });

       var orcamento = new Orcamento
        {
        Nome = request.Nome,
        Telefone = request.Telefone,
        Email = request.Email,
        TipoImovel = request.TipoImovel,
        Cep = request.Cep,               // ✅ novo
        Cidade = request.Cidade,
        CidadeNome = request.CidadeNome, // ✅ novo
        Urgencia = request.Urgencia,
        Mensagem = request.Mensagem,
        Servicos = string.Join(",", request.Servicos ?? new List<string>())

            // ✅ converte a lista ["CFTV","Alarme"] para "CFTV,Alarme"
        };

        _service.Adicionar(orcamento);

        return Ok(new { mensagem = "Orçamento criado com sucesso", id = orcamento.Id });
    }

    [HttpGet]
    public IActionResult Listar()
    {
        return Ok(_service.Listar());
    }
}

// ───────────────────────────────
// REQUEST (o que vem do front)
// ───────────────────────────────
public class OrcamentoRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? TipoImovel { get; set; }
    public string? Cep { get; set; }        // ✅ novo
    public string? Cidade { get; set; }
    public string? CidadeNome { get; set; } // ✅ novo
    public string? Urgencia { get; set; }
    public string? Mensagem { get; set; }
    public List<string>? Servicos { get; set; }
}