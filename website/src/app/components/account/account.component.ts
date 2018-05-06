import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Api } from '../../services/api';
import { AppService } from '../../services/appService';
import { Router } from '@angular/router';

declare var bootbox:any;
@Component({
    selector: 'account',
    templateUrl: './account.component.html',
    styleUrls: ['./account.component.css']
})
export class AccountComponent {
    constructor(public _api: Api, public _appService: AppService, public _router: Router) {
        this.appService = _appService;

        //do any operations which are dependent upon a valid user session.
        this.appService.UserLoaded.subscribe(data => {
            if (data == true)
                this.getCurrentUser();
        });

    }
    email: string;
    name: string;
    currentPassword: string;
    newPassword: string;
    confirmPassword: string;
    appService: AppService;


    saveUserChanges() {
        this._api.updateUserById(this._appService.User.id, this.email, this.name).subscribe(data => {

            bootbox.alert('Your account changes have been saved');

            //Re-fetch the current user data. Exact same process as when the application first loads.
            this._api.getCurrentUser().subscribe(
                data => {
                    this.appService.User = data.json();
                    this.appService.UserLoaded.next(true);
                },
                error => {
                    this._router.navigate(['/signin']);
                });
        },
        error => {
            bootbox.alert('Error: ' + error.text());
        });
    }

    deleteUser() {
        bootbox.confirm({
            message: 'Are you certain you want to delete your account?',
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-danger'
                }
            },
            callback: (result) => {
                if(result == true)
                {
                    this._api.deleteUserById(this._appService.User.id).subscribe(data => {
                        this._api.logoutUser().subscribe(data => {
                            this.appService.User = null;
                            this._router.navigate(['/signin']);
                        })
                    });
                }
            }
        });
    }

    changeUserPassword() {
        this._api.changeUserPasswordById(this._appService.User.id, this.currentPassword, this.newPassword, this.confirmPassword).subscribe(data => {
            bootbox.alert('Your password has been changed');
        },
        error => {
            bootbox.alert('Error: ' + error.text());
        });
    }

    getCurrentUser() {
        this._api.getCurrentUser().subscribe(data => {
            this.name = data.json().name;
            this.email = data.json().email;
        });
    }

}
