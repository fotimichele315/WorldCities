import { Component, OnInit, OnDestroy } from '@angular/core';
import {  Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import {  Subject, takeUntil  } from 'rxjs';

@Component({
  selector: 'app-nav-menu',
  standalone: false,
  templateUrl: './nav-menu.component.html',
  styleUrl: './nav-menu.component.scss'
})
export class NavMenuComponent implements OnInit, OnDestroy {


  private destroySubject = new Subject();
  isLoggedIn: boolean = false;

  constructor(
    private authServices: AuthService,
    private router: Router
  ) {
    this.authServices.authStatus
      .pipe(takeUntil(this.destroySubject))
      .subscribe(result => {
        this.isLoggedIn = result;
      });
  }


  get isLoginPage(): boolean {
    return this.router.url === '/login';
  }


  onLogout(): void {

    this.authServices.logout();
    this.router.navigate(["/logout"]);
  }
  ngOnInit(): void {
    this.isLoggedIn = this.authServices.isAuthenticated();
  }

  ngOnDestroy(){
    this.destroySubject.next(true);
    this.destroySubject.complete();
  }


}
