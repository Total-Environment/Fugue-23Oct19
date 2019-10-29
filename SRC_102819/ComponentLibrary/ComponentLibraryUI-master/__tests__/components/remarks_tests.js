import { Remarks } from '../../src/components/checklists/remarks';
import chai, { expect } from 'chai';
import {shallow} from 'enzyme';
import React from 'react';
import chaiEnzyme from 'chai-enzyme';

chai.use(chaiEnzyme());

describe('Remarks', () => {
   it('remarks should render table of data', () => {
       let wrapper = shallow(<Remarks/>);
       expect(wrapper).to.contain('Remarks | Corrective Action:');
       expect(wrapper).to.contain('Instruments Required:');
   });
});
