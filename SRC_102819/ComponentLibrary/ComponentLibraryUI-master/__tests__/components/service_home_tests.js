import React from 'react';
import {shallow} from 'enzyme';
import sinonChai from 'sinon-chai';
import chai, {expect} from 'chai';
import chaiEnzyme from 'chai-enzyme';

import {ServiceHome} from '../../src/components/service-home'

chai.use(sinonChai);
chai.use(chaiEnzyme());

describe("Service Home",()=>{
    it("should render search",()=>{
        let wrapper = shallow(<ServiceHome />);
        // expect(wrapper).to.be.descendants('SearchConnector');
    })
});
