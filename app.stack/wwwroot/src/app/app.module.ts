import { BrowserModule } from '@angular/platform-browser';
import { NgModule, Injectable } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule, CanActivate, Router } from '@angular/router';
import { Api } from './services/api';
import { AppService } from './services/appService';

import { AppComponent } from './components/app/app.component';
import { HomeComponent } from './components/home/home.component';
import { TodoComponent } from './components/todo/todo.component';
import { AccountComponent } from './components/account/account.component';
import { RegisterComponent } from './components/register/register.component';
import { SignInComponent } from './components/signin/signin.component';

@Injectable()
export class AuthGuard implements CanActivate {
    constructor(public _router: Router, public _appService: AppService) {
    }
    canActivate() {

        var isLoggedIn = localStorage.getItem('authenticated');
        if (isLoggedIn !== 'true') {
            this._appService.User = null;
            localStorage.removeItem('authenticated');
            this._router.navigate(['/signin']);
            return false;
        }
        else {
            return true;
        }
    }
}

@NgModule({
    declarations: [
        AppComponent,
        RegisterComponent,
        SignInComponent,
        TodoComponent,
        AccountComponent,
        HomeComponent
    ],
    imports: [
        BrowserModule,
        CommonModule,
        HttpModule,
        FormsModule,
        RouterModule.forRoot([
            { path: 'home', redirectTo: '', pathMatch: 'full' },
            { path: '', component: HomeComponent },
            { path: 'register', component: RegisterComponent },
            { path: 'signin', component: SignInComponent },
            { path: 'todo', component: TodoComponent, canActivate: [AuthGuard] },
            { path: 'account', component: AccountComponent, canActivate: [AuthGuard] },
            { path: '**', redirectTo: '' }
        ])
    ],
    providers: [
        AuthGuard,
        Api,
        AppService
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
