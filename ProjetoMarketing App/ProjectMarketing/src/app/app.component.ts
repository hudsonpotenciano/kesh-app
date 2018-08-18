import { Component, ViewChild } from '@angular/core';
import { Platform, Nav } from 'ionic-angular';
import { StatusBar } from '@ionic-native/status-bar';
import { SplashScreen } from '@ionic-native/splash-screen';

import { TranslateService } from '@ngx-translate/core';

@Component({
  templateUrl: 'app.html'

})
export class MyApp {

  rootPage: any = "IntroducaoPage";
  @ViewChild(Nav) nav: Nav;

  constructor(

    platform: Platform,
    statusBar: StatusBar,
    splashScreen: SplashScreen,
    private translate: TranslateService) {

    platform.ready().then(() => { 
      statusBar.styleLightContent();
      statusBar.overlaysWebView(false);
      statusBar.backgroundColorByHexString("#fff");
      splashScreen.hide();
    });

    this.initTranslate();

    localStorage.clear();
  }

  initTranslate() {
    // Definir o idioma padrão para as sequências de tradução e o idioma atual.
    this.translate.setDefaultLang('pt-br');

    this.translate.use('pt-br');
    // mudar a linguagem de acordo com a linguagem selecionada nas configuracoes 
  }

  openPage(page) {
    this.nav.setRoot(page.component);
  }
}