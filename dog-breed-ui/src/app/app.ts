import { HttpClient } from '@angular/common/http';
import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  constructor(private http: HttpClient) {
  }

  onFileSelected(event: Event) {
  const input = event.target as HTMLInputElement;
  const file = input.files?.[0];

  if (!file) {
    return;
  }

  const formData = new FormData();
  formData.append('file', file);

  this.http.post(
    'http://localhost:5072/api/dogs/upload',
    formData
  ).subscribe(response => {
    console.log(response);
  });
}
}
