import {mapStateToProps} from '../src/material_home_connector';
import chai, {expect} from 'chai';
import sinonChai from 'sinon-chai';

chai.use(sinonChai);

describe('Material Home Connector', () => {
    describe('mapStateToProps', () => {
        it('should transform state and props',()=>{
            const state = {reducer:{materialGroupSearchDetails:{groups: ['Clay materials', 'Cement materials'], response: 'we found teakwood in'}}};
            const props = {};
            const expectedOutput = {searchResponse:'we found teakwood in', groups:['Clay materials', 'Cement materials']};
            const output = mapStateToProps(state,props);
            expect(output).to.deep.equal(expectedOutput);
        });
    });
});
