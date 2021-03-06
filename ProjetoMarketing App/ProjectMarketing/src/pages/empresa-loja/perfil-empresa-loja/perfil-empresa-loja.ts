import { Component, ViewChild } from '@angular/core';
import { IonicPage, ModalController, Slides, NavController } from 'ionic-angular';
import { StorageEmpresaProvider } from '../../../providers/storage/storage-empresa';
import { DadosEmpresaLoja, ImagemCatalogo, AtualizaPerfilModel } from '../../../models/empresa.model';
import { EmpresaLojaProvider } from '../../../providers/empresa-loja/empresa-loja';
import { UtilitariosProvider } from '../../../providers/utilitarios/utilitarios';
import { DomSanitizer } from '@angular/platform-browser';

@IonicPage()
@Component({
  selector: 'page-perfil-empresa-loja',
  templateUrl: 'perfil-empresa-loja.html',
})
export class PerfilEmpresaLojaPage {

  @ViewChild('fileInput') fileInput;
  @ViewChild('slides') Slides: Slides;

  imagensCatalogo: ImagemCatalogo[] = [];
  dadosEmpresa: DadosEmpresaLoja;
  segment: string = "cadastro";

  constructor(private storageEmpresa: StorageEmpresaProvider,
    private empresaLojaProvider: EmpresaLojaProvider,
    private sanitizer: DomSanitizer,
    private modalCtrl: ModalController,
    private navCtrl: NavController,
    private utilitarios: UtilitariosProvider) {
    this.sanitizer;
    this.dadosEmpresa = this.storageEmpresa.recupereDadosEmpresaLoja()
    this.imagensCatalogo = this.dadosEmpresa.Catalogo;
  }

  selecioneLocalizacao() {
    var modal = this.modalCtrl.create("SelecaoLocalizacaoPage", {
      lat: this.dadosEmpresa.Perfil.Latitude,
      long: this.dadosEmpresa.Perfil.Longitude
    }, { enableBackdropDismiss: false });

    modal.present();

    modal.onDidDismiss((coodenadas) => {

      if (!coodenadas) return;

      this.dadosEmpresa.Perfil.Latitude = coodenadas.lat;
      this.dadosEmpresa.Perfil.Longitude = coodenadas.lng;
    });
  }

  selecioneImagem() {
    this.fileInput.nativeElement.click();
  }

  aoEscolherImagem(event: any) {
    let reader = new FileReader();
    reader.onloadend = (readerEvent) => {
      if (!this.imagensCatalogo[this.imagensCatalogo.length])
        this.imagensCatalogo[this.imagensCatalogo.length] = new ImagemCatalogo();

      this.utilitarios.getBase64Image((readerEvent.target as any).result, (imagem) => {
        this.imagensCatalogo[this.imagensCatalogo.length - 1].Imagem = imagem;
        setTimeout(() => {
          this.Slides.slideNext();
          this.utilitarios.removaAlertaCarregando();
        }, 2000);
      });
    };
    
    if (event.target.files.length > 0) {
      this.utilitarios.mostreAlertaCarregando("Adicionando a imagem selecionada, aguarde um instante.");
      reader.readAsDataURL(event.target.files[0]);
    }
    else {
      this.utilitarios.removaAlertaCarregando();
    }
  }

  removaImagem() {
    this.imagensCatalogo.splice(this.Slides.getActiveIndex(), 1);
    this.Slides.slidePrev();
  }

  salvar() {
    let perfil: AtualizaPerfilModel = new AtualizaPerfilModel();
    perfil.Descricao = this.dadosEmpresa.Perfil.Descricao;
    perfil.Telefone = this.dadosEmpresa.Perfil.Telefone;
    perfil.Telefone2 = this.dadosEmpresa.Perfil.Telefone2;
    perfil.IdPerfilEmpresa = this.dadosEmpresa.Perfil.IdPerfilEmpresa;
    perfil.Catalogo = this.imagensCatalogo;
    perfil.Latitude = this.dadosEmpresa.Perfil.Latitude;
    perfil.Longitude = this.dadosEmpresa.Perfil.Longitude;
    this.utilitarios.mostreAlertaCarregando("Salvando dados da loja, aguarde um instante.");
    this.empresaLojaProvider.atualizePerfilEmpresa(perfil)
      .then(() => {
        this.storageEmpresa.armazeneDadosEmpresaLoja(this.dadosEmpresa);
        this.utilitarios.removaAlertaCarregando();
        this.navCtrl.pop();
        this.utilitarios.mostreMensagemSucesso("Dados salvos com sucesso");
      }).catch((retorno) => {
        retorno;
        this.navCtrl.pop();
        this.utilitarios.mostreMensagemErro("Houve um erro ao salvar os dados, tente novamente");
        this.utilitarios.removaAlertaCarregando();
      })
  }

  voltar() {
    this.navCtrl.pop();
  }
}
