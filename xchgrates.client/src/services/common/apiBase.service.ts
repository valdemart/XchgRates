import { HttpClient, HttpErrorResponse, HttpResponse } from "@angular/common/http";
import { inject } from "@angular/core";
import { catchError, firstValueFrom, map, of, retry, timer } from "rxjs";
import { ApiResponse } from "./apiResponse";
import { NotificationsService } from "../notifications/notifications.service";

export class ApiBase {
  private http: HttpClient = inject(HttpClient);
  private notificationsService: NotificationsService = inject(NotificationsService);

  protected async get<T>(url: string): Promise<T | null> {
    return await firstValueFrom(this.http.get<ApiResponse<T>>(url, { observe: "response" })
      .pipe(
        retry({
          count: 2, //max 3 attempts: initial request + 2 retries
          delay: (error: HttpErrorResponse, retryCount: number) => {
            if (error.error?.success !== null && error.error?.success !== undefined) {
              //intentional error code that has a message - don't retry
              throw error;
            }

            console.error(`Retry ${retryCount} with error ${error.error ?? error.message}`);
            return timer(1000);
          },
          resetOnSuccess: true
        }),

        map((response: HttpResponse<ApiResponse<T>>) => {
          if (!response.body) {

            switch (response.status) {
              case 204:
                this.notificationsService.pushGood("Brak wystarczających danych o kursach");
                break;
              default:
                this.notificationsService.pushBad("Api error. Api returned empty body.");
            }

            return null;
          }

          if (response.body.success === false) {
            this.notificationsService.pushBad(response.body.error ?? `API error occured (status ${response.status})`);
            return null;
          }

          return response.body.result;
        }),

        catchError((error: HttpErrorResponse) => {
          const result = error.error;

          if (result?.success === true) {
            this.notificationsService.pushGood(result.error);
          } else if (result?.success === false) {
            this.notificationsService.pushBad(result.error);
          } else {
            this.notificationsService.pushBad(`HTTP error ${error.status}: ${error.error ?? error.message}`);
          }

          return of(null);
        })
      ));
  }
}
