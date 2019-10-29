import chai, { expect } from 'chai';
import { GtcFooter } from '../../src/components/checklists/gtc_footer';
import {shallow} from 'enzyme';
import React from 'react';
import chaiEnzyme from 'chai-enzyme';

chai.use(chaiEnzyme());

describe('GtcFooter', () => {
   it('should render footer data', () => {
       const wrapper = shallow(<GtcFooter/>);
       expect(wrapper).to.contain(<b>Vendors</b>);
       expect(wrapper).to.contain(<b>Customers</b>);
       expect(wrapper).to.contain(<b>Arel Solutions India Private Limited</b>);
       expect(wrapper).to.contain(<b>Total Environment Building Systems Private Limited</b>);
   });
});
