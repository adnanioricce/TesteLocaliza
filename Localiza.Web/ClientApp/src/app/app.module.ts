import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule,ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CreateEditModalComponent  } from "./components/create-edit-modal/app-cobranca-modal.component";
import { CobrancasTableComponent } from './components/cobrancas-table/cobrancas-table.component'
import { ClienteTableComponent } from './components/cliente-table/cliente-table.component';
import { LoginComponent  } from "./components/login/login.component";
import { SignupComponent  } from "./components/signup/signup.component";
import { AuthService } from "./services/auth.service"
import "bootstrap"
@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    // CounterComponent,
    // FetchDataComponent,
    CobrancasTableComponent,
    ClienteTableComponent,
    LoginComponent,
    SignupComponent,
    CreateEditModalComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      // { path: 'counter', component: CounterComponent },
      // { path: 'fetch-data', component: FetchDataComponent },
      { path: 'clientes', component: ClienteTableComponent },
      { path: 'cobrancas/:id', component: CobrancasTableComponent },
      { path: 'login', component: LoginComponent },
      { path: 'signup', component: SignupComponent }
    ])
  ],
  providers: [AuthService],
  bootstrap: [AppComponent]
})
export class AppModule { }
