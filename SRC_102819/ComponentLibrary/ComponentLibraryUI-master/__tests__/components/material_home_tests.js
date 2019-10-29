import React from 'react';
import {shallow} from 'enzyme';
import sinonChai from 'sinon-chai';
import chai, {expect} from 'chai';
import chaiEnzyme from 'chai-enzyme';

import {MaterialHome} from '../../src/components/material-home'

chai.use(sinonChai);
chai.use(chaiEnzyme());

describe("Material Home",()=>{
    it("should render search",()=>{
        let wrapper = shallow(<MaterialHome />);
        // expect(wrapper).to.be.descendants('SearchConnector');
    })


});
