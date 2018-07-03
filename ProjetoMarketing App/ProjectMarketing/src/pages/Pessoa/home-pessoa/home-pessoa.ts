import { Component } from '@angular/core';
import { IonicPage, NavController, NavParams } from 'ionic-angular';
import { PessoaProvider } from '../../../providers/pessoa/pessoa';
import { Empresa } from '../../../models/empresa.model';

@IonicPage()
@Component({
  selector: 'page-home-pessoa',
  templateUrl: 'home-pessoa.html',
})

export class HomePessoaPage {

  empresas: Empresa[] = [];

  constructor(
    public navCtrl: NavController,
    public navParams: NavParams,
    private pessoaProvider: PessoaProvider) {

  }

  ionViewDidEnter() {
    this.obtenhaEmpresas();
  }

  obtenhaEmpresas() {
    this.pessoaProvider.ObtenhaEmpresas()
      .then((empresas: Empresa[]) => {
        this.empresas = empresas;
      })
  }

  abraPerfilEmpresa(empresa: Empresa) {
    this.navCtrl.push("PerfilEmpresaPage", empresa);
  }
}