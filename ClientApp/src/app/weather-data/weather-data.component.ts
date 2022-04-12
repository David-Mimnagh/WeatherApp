import { Component, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-weather-data',
  templateUrl: './weather-data.component.html'
})
export class WeatherDataComponent {
    public forecasts: WeatherForecast[];
    public citiesList: CityCondensed[];
    httpClient: HttpClient;
    baseUrl: string;
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
      this.httpClient = http;
      this.baseUrl = baseUrl;
    }
    getWeatherForecast(latitude: number = null, longitude: number = null, location: string = null) {
        if (latitude && !longitude || !latitude && longitude) {
            throw new Error("Please provide both a latitute and longitude.")
        }
        let queryParams = new HttpParams();

        if (latitude && longitude) {
            queryParams = queryParams.append("latitude", latitude.toString());
            queryParams = queryParams.append("longitude", longitude.toString());
        }

        if (location) {
            queryParams = queryParams.append("location", location);
        }

        this.httpClient.get<WeatherForecast[]>(this.baseUrl + 'WeatherForecast/GetWeatherInfo', { params: queryParams }).subscribe(result => {
            this.forecasts = result;
        }, error => console.error(error));
    }
    getCurrentLocation() {
        navigator.geolocation.getCurrentPosition((geoLoc) => this.getWeatherForecast(geoLoc.coords.latitude, geoLoc.coords.longitude)) 
    }
    getCities(location: string = null) {
        this.httpClient.get<CityCondensed[]>(this.baseUrl + 'City/GetCities', { params: { location } }).subscribe(result => {
            this.citiesList = result;
        }, error => console.error(error));
    }
}
interface WeatherForecast {
    date: string;
    temperatureC: number;
    temperatureF: number;
    summary: string;
}
interface CityCondensed {
    Key: string;
    Name: number;
}
