import { NgModule } from '@angular/core';
import { IonicPageModule } from 'ionic-angular';
import { LoginEmpresaPage } from './login-empresa';
import { EmpresaProvider } from '../../../providers/empresa/empresa';
import { StorageEmpresaProvider } from '../../../providers/storage/storage-empresa';

@NgModule({
  declarations: [
    LoginEmpresaPage,
  ],
  imports: [
    IonicPageModule.forChild(LoginEmpresaPage),
  ],
  providers:
    [
      EmpresaProvider,
      StorageEmpresaProvider
    ] 
})
export class LoginEmpresaPageModule {}
