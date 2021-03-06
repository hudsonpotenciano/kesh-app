export class Enumerador {
    Codigo: number;
    Descricao: number;

    constructor(_codigo, _descricao) {
        this.Codigo = _codigo;
        this.Descricao = _descricao;
    }
}

export class EnumeradorDeCacheStorageEmpresaLoja {

    obtenhaDadosEmpresaLoja = new Enumerador(1, "obtenhaDadosEmpresaLoja");

    static obtenhaTodos(): Enumerador[] {
        return [
            new EnumeradorDeCacheStorageEmpresaLoja().obtenhaDadosEmpresaLoja
        ]
    }
}

export class EnumeradorDeCacheStorageEmpresa {

    obtenhaDadosEmpresaAdmin = new Enumerador(1, "obtenhaDadosEmpresaAdmin");
    obtenhaPerfisEmpresa = new Enumerador(2, "obtenhaPerfisEmpresa");

    static obtenhaTodos(): Enumerador[] {
        return [
            new EnumeradorDeCacheStorageEmpresa().obtenhaDadosEmpresaAdmin,
            new EnumeradorDeCacheStorageEmpresa().obtenhaPerfisEmpresa]
    }
}

export class EnumeradorDeCacheStorageTransacoes {
    obtenhaCuponsEVendasEmpresaAdmin = new Enumerador(1, "obtenhaCuponsEVendasEmpresaAdmin");
    obtenhaCuponsEVendasEmpresa = new Enumerador(2, "obtenhaCuponsEVendasEmpresa");
    obtenhaCuponsEVendasPessoa = new Enumerador(3, "obtenhaCuponsEVendasPessoa");
    obtenhaCuponsEVendasPessoaEmpresa = new Enumerador(4, "obtenhaCuponsEVendasPessoaEmpresa");


    static obtenhaTodos(): Enumerador[] {
        return [
            new EnumeradorDeCacheStoragePessoa().obtenhaDadosPessoaLojas,
            new EnumeradorDeCacheStoragePessoa().obtenhaPessoaEPerfilEmpresas,
            new EnumeradorDeCacheStoragePessoa().obtenhaPessoasCompartilhamento
        ]
    }
}

export class EnumeradorDeCacheStoragePessoa {
    obtenhaDadosPessoaLojas = new Enumerador(1, "obtenhaDadosPessoaLojas");
    obtenhaPessoaEPerfilEmpresas = new Enumerador(2, "obtenhaPessoaEPerfilEmpresas");
    obtenhaPessoasCompartilhamento = new Enumerador(3, "obtenhaPessoasCompartilhamento");
    ObtenhaComentarioENotaPessoasEmpresas = new Enumerador(4, "ObtenhaComentarioENotaPessoasEmpresas");
    ObtenhaDadosPessoa = new Enumerador(5, "ObtenhaDadosPessoa");

    static obtenhaTodos(): Enumerador[] {
        return [
            new EnumeradorDeCacheStoragePessoa().obtenhaDadosPessoaLojas,
            new EnumeradorDeCacheStoragePessoa().obtenhaPessoaEPerfilEmpresas,
            new EnumeradorDeCacheStoragePessoa().obtenhaPessoasCompartilhamento,
            new EnumeradorDeCacheStoragePessoa().ObtenhaComentarioENotaPessoasEmpresas,
            new EnumeradorDeCacheStoragePessoa().ObtenhaDadosPessoa]
    }
}

export class EnumeradorDeCategorias {
    AlimentosEBebidas = new Enumerador(1, "Alimentos e Bebidas");
    Animais = new Enumerador(2, "Animais");
    Automoveis = new Enumerador(3, "Automóveis");
    BrinquedosEGames = new Enumerador(4, "Games e brinquedos");
    CasaEJardim = new Enumerador(5, "Casa e Jardim");
    Educacao = new Enumerador(6, "Educação");
    EsporteELazer = new Enumerador(7, "Esporte e Lazer");
    Entreterimento = new Enumerador(8, "Entreterimento");
    EsteticaEBeleza = new Enumerador(9, "Estética e Beleza");
    Informatica = new Enumerador(10, "Informática");
    ModaEAcessorios = new Enumerador(11, "Moda e Acessórios");
    Saude = new Enumerador(12, "Saúde");
    Servicos = new Enumerador(13, "Serviços");
    Outros = new Enumerador(14, "Outros");

    obtenhaTodos(): Enumerador[] {
        return [
            this.AlimentosEBebidas,
            this.Educacao,
            this.CasaEJardim,
            this.EsporteELazer,
            this.EsteticaEBeleza,
            this.Informatica,
            this.Saude,
            this.Servicos,
            this.Automoveis,
            this.BrinquedosEGames,
            this.Entreterimento,
            this.ModaEAcessorios,
            this.Animais,
            this.Outros]
    }
}
