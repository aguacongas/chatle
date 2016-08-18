import { AppComponent } from "./app.component";

describe('AppComponent', () => {
    it("app component should exist", () => {
        let component = new AppComponent();
        expect(component).toBeDefined();
    });
});