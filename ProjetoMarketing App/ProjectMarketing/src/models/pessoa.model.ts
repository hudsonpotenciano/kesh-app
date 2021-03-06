import { Empresa, Perfil, ImagemCatalogo, Conta } from "./empresa.model";
import { Cupom } from "./models.model";
import { Data } from "../pipes/pipes/pipes";

export class CadastroPessoaModel {
    Nome: string;
    Email: string;
    Senha: string;
    Foto: any;
}

export class CadastroPessoaRedeSocialModel {
    Nome: string;
    Email: string;
    Id: string;
    Foto: any;
}

export class Pessoa {
    IdPessoa: string;
    Nome: string;
    Email: string;
    Latitude: number;
    Longitude: number;
    urlImagem: string;
}

export class PessoaEmpresaVenda {
    PontuacaoDinheiro: number;
    IdPessoa: string;
    IdEmpresa: string;
}

export class PessoaEmpresa {
    Empresa: Empresa;
    Catalogo: ImagemCatalogo[];
    Comentario: string;
    Nota: number;
    NotaGeral: number;
    IdPessoa: string;
}

export class DadosPessoaEmpresa {
    Empresa: Empresa;
    Perfil: Perfil;
    PessoaEmpresa: PessoaEmpresa;
    Conta: Conta;
    NotaGeral: number;
    Distancia: string;
    Catalogo: ImagemCatalogo[];
}

export enum UnidadeDeMedidaLocalizacao {
    Kilometros,
    Milhas
}
export class PessoaLoja {
    Loja: Perfil;
    Pontos: number;
    NomeEmpresa: string;
    PontosEmDinheiro: number;
}

export class DTOCupomParaVenda {
    Cupom: Cupom;
    TotalDinheiroPessoa: number;
}

export class Compartilhamento {
    GuidCompartilhamento: string;
    IdCompartilhamento: number;
    Data: Data;
}