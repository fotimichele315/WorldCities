import { Component , OnInit} from '@angular/core';
import { AuthService } from './auth/auth.service';
import { environment } from '../environments/environment';
import { Observable, fromEvent, merge, map, startWith } from 'rxjs';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  title = "WorldCities";

  // Observable che emette true se online, false se offline
  public online$: Observable<boolean> = merge(
    fromEvent(window, 'online').pipe(map(() => true)),
    fromEvent(window, 'offline').pipe(map(() => false))
  ).pipe(
    startWith(navigator.onLine)
  );

  constructor(private authservice: AuthService) {

  }

  ngOnInit(): void {
    this.authservice.init();
  }

}
