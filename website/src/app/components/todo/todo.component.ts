import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Api } from '../../services/api';
import { AppService } from '../../services/appService';
declare var bootbox:any;


@Component({
  selector: 'todo',
  templateUrl: './todo.component.html',
  styleUrls: ['./todo.component.css']
})
export class TodoComponent {
  constructor(public _api: Api, public _appService: AppService) {
    this.appService = _appService;
    this.tasks = new Array<any>();

    //do any operations which are dependent upon a valid user session.
    this.appService.UserLoaded.subscribe(data => {
      if (data == true)
        this.getTodos();
    });

  }
  model: string;
  tasks: Array<any>;
  appService: AppService;

  createTodo() {
    this._api.createTodoByUser(this.appService.User.id, this.model).subscribe(data => {
      this.model = '';
      this.tasks.push(data.json());
    });
  }

  completeTodo(item: any) {
    item.completed = item.completed ? false : true;
    this._api.completeTodoById(this.appService.User.id, item.id, item.completed).subscribe(data => { });
  }

  deleteTodo(item: any) {

    bootbox.confirm({
      message: 'Are you certain you want to delete this item?',
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
        if (result == true) {
          this._api.deleteTodoById(this.appService.User.id, item.id).subscribe(data => {
            this.tasks = this.tasks.filter(i => i.id != item.id);
          });
        }
      }
    });

  }

  getTodos() {
    this._api.getTodosByUserId(this.appService.User.id).subscribe(data => {
      this.tasks = data.json();
    });
  }

}
