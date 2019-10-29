import chai, { expect } from 'chai';
import { Footer } from '../../src/components/checklists/footer';
import {shallow} from 'enzyme';
import React from 'react';
import chaiEnzyme from 'chai-enzyme';

chai.use(chaiEnzyme());

describe('Footer', () => {
   it('render should render data', () => {
       let wrapper = shallow(<Footer/>);
       expect(wrapper).to.contain(<b>Stores Executive</b>);
       expect(wrapper).to.contain(<b>Stores Manager</b>);
       expect(wrapper).to.contain('Name:');
       expect(wrapper).to.contain('Sign & Date:');
   }) ;
});
