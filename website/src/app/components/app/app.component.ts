import { Component, ViewEncapsulation, OnInit } from '@angular/core';
import { Api } from '../../services/api';
import { AppService } from '../../services/appService';
import { Router } from '@angular/router';
import { Observable, Subject } from 'rxjs';

//import ViewEncapsulation and set encapsulation to ViewEncapsulation.None - this enables global css styles
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  constructor(public _api: Api, public _router: Router, public _appService: AppService) {
    this.appService = _appService;
  }

  ngOnInit(): void {
    this._api.getCurrentUser().subscribe(
      data => {
        this.appService.User = data.json();
        this.appService.UserLoaded.next(true);
				localStorage.setItem('authenticated', 'true');
      },
      error => {
        //this._router.navigate(['/signin']);
      });
  }

  public appService: AppService;

  logout() {
    this._api.logoutUser().subscribe(data => {
      this.appService.User = null;
				localStorage.removeItem('authenticated');
        this._router.navigate(['/signin']);
    });
  }

}
