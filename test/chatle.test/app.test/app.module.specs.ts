import { AppModule } from "../../../src/chatle/app/app.module"
describe('AppModule', () => {
    it('exists', () => {
        let m = new AppModule();
        expect(m).toBeDefined();
    });
});