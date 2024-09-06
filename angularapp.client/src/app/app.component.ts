import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  public forecasts: any[] = [];

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.getForecasts();
  }

  getForecasts() {
    this.http.get<any[]>('/api/employees').subscribe(
      (result) => {
        console.log('employees api called');
        this.forecasts = result;
      },
      (error) => {
        console.error(error);
      }
    );
  }

  title = 'angularapp.client';
}
