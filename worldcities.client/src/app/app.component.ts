import { Component , OnInit} from '@angular/core';
import { AuthService } from './auth/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  title = "WorldCities";


  constructor(private authservice: AuthService) {

  }

  ngOnInit(): void {
    this.authservice.init();
  }

}
