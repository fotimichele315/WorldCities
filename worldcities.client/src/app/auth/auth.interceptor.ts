
import { Injectable } from '@angular/core';
import { HttpInterceptor,HttpClient, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
 import { Observable, throwError, catchError } from 'rxjs';
import {  Router } from '@angular/router';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class AuthInterceptor implements HttpInterceptor {

  constructor(private authService: AuthService, private router: Router) {
  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    //get the auth token
    var token = this.authService.getToken();

    //if the token is present, clone the request
    if (token) {
      req = req.clone({
        setHeaders: {
          Authorization: "Bearer " + token
        }
      })
    }
    //send the requet to next handler
    return next.handle(req).pipe(

      catchError((error) => {
        //perform logout on 401 - Umhauthorizated HTTp response errors
        if (error instanceof HttpErrorResponse && error.status === 401) {
          this.authService.logout();
          this.router.navigate(['login'])
        }
        return throwError(() => error);
      })


    );
  }
}
