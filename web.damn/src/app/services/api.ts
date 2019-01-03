import { Injectable } from '@angular/core';
import { Http, RequestOptions } from '@angular/http';

@Injectable()
export class Api {
    private apiRoot : string;
    constructor(public _client: Http) {
        if(window.location.host == 'localhost') {
            this.apiRoot = '//localhost/api'; //For Local Docker
        }
        else if (window.location.host == 'localhost:4200') {
            this.apiRoot = '//localhost:50857/api' //For Development
        }
        else {
            this.apiRoot = '//' + window.location.host + '/api' //For Production
        }
    }
    options = new RequestOptions({withCredentials: true});

    public registerUser(email: string, password: string, confirmPassword: string) {
        let user = { email, password, confirmPassword };
        return this._client.post(this.apiRoot + '/v1/user', user, this.options)
    }

    public authenticateUser(email: string, password: string) {
        let user = { email, password };
        return this._client.post(this.apiRoot + '/v1/authenticate', user, this.options);
    }

    public logoutUser() {
        return this._client.get(this.apiRoot + '/v1/logout', this.options);
    }

    public getCurrentUser() {
        return this._client.get(this.apiRoot + '/v1/user/me', this.options);
    }

    public deleteUserById(userId: string) {
        return this._client.delete(this.apiRoot + '/v1/user/' + userId, this.options);
    }

    public updateUserById(userId: string, email: string, name: string) {
        let content = { email, name };
        return this._client.put(this.apiRoot + '/v1/user/' + userId, content, this.options);
    }

    public changeUserPasswordById(userId: string, oldPassword: string, newPassword: string, confirmPassword: string) {
        let content = { oldPassword, newPassword, confirmPassword };
        return this._client.put(this.apiRoot + '/v1/user/' + userId + '/password', content, this.options);
    }

    public createTodoByUser(userId: string, task: string) {
        let content = { task };
        return this._client.post(this.apiRoot + '/v1/user/' + userId + '/todo', content, this.options);
    }

    public getTodosByUserId(userId: string) {
        return this._client.get(this.apiRoot + '/v1/user/' + userId + '/todo', this.options);
    }

    public completeTodoById(userId: string, todoId: string, status: boolean) {
        return this._client.put(this.apiRoot + '/v1/user/' + userId + '/todo/' + todoId + '/status/' + status, null, this.options);
    }

    public deleteTodoById(userId: string, todoId: string) {
        return this._client.delete(this.apiRoot + '/v1/user/' + userId + '/todo/' + todoId, this.options);
    }

}
