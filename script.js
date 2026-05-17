document.addEventListener('DOMContentLoaded', () => {

  // ================= MENU =================
  const menuToggle = document.getElementById('menuToggle');
  const navList = document.getElementById('nav-list');

  if (menuToggle && navList) {
    menuToggle.addEventListener('click', (e) => {
      e.preventDefault();
      navList.classList.toggle('open');
    });

    navList.querySelectorAll('a').forEach(link => {
      link.addEventListener('click', () => {
        navList.classList.remove('open');
      });
    });
  }

  // ================= BOTÃO TOPO =================
  const backToTop = document.getElementById('backtotop');

  if (backToTop) {
    backToTop.addEventListener('click', (e) => {
      e.preventDefault();
      window.scrollTo({ top: 0, behavior: 'smooth' });
    });
  }

  // ================= SCROLL SUAVE =================
  document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
      const target = document.querySelector(this.getAttribute('href'));
      if (target) {
        e.preventDefault();
        target.scrollIntoView({ behavior: 'smooth' });
      }
    });
  });

  // ================= ANIMAÇÕES =================
  const animatedElements = document.querySelectorAll(
    '.service-card, .service-card-last, .feature-item, .contact-block, .partner-item, .sobre-visual, .sobre-content, .orcamento-card, .orcamento-side, .contact-phone'
  );

  if ('IntersectionObserver' in window && animatedElements.length) {
    const observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          entry.target.classList.add('visible');
          observer.unobserve(entry.target);
        }
      });
    }, { threshold: 0.12 });

    animatedElements.forEach((el, i) => {
      el.classList.add('fade-in');
      el.style.transitionDelay = `${i * 0.07}s`;
      observer.observe(el);
    });
  } else {
    animatedElements.forEach(el => el.classList.add('visible'));
  }

  // ================= AUTO SELEÇÃO DE SERVIÇO =================
  const params = new URLSearchParams(window.location.search);
  const servico = params.get('servico');

  if (servico) {
    const checkbox = document.querySelector(`input[name="servicos"][value="${servico}"]`);
    if (checkbox) checkbox.checked = true;
  }

  // ================= FORMULÁRIO =================
  const form = document.getElementById('formOrcamento');

  if (!form) return;

  form.addEventListener('submit', async (e) => {
    e.preventDefault();

    const servicos = Array.from(
      document.querySelectorAll('input[name="servicos"]:checked')
    ).map(item => item.value);

    const dados = {
      nome: document.getElementById('nome')?.value.trim(),
      telefone: document.getElementById('telefone')?.value.trim(),
      email: document.getElementById('email')?.value.trim(),
      tipoImovel: document.getElementById('tipo-imovel')?.value,
      cidade: document.getElementById('cidade')?.value.trim(),
      urgencia: document.getElementById('urgencia')?.value,
      mensagem: document.getElementById('mensagem')?.value.trim(),
      servicos: servicos
    };

    console.log("Enviando dados:", dados);

    try {
      const response = await fetch('http://localhost:5193/api/orcamentos', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(dados)
      });

      let resultado = {};
      try {
        resultado = await response.json();
      } catch {}

      if (!response.ok) {
        alert(resultado.mensagem || 'Erro ao salvar orçamento.');
        return;
      }

      alert('Orçamento enviado com sucesso!');
      form.reset();

    } catch (error) {
      console.error("Erro:", error);
      alert('Erro de conexão com o servidor.');
    }
  });

});